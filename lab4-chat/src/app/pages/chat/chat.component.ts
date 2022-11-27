import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MessageDecodedData, MessageEncodedData } from 'src/app/models/responses/IMessageData';
import { ChatService } from 'src/app/services/chat.service';
import { SignalRService } from 'src/app/services/signal-r.service';
import * as BigInteger from 'jsbn';
import * as CryptoJS from 'crypto-js';
import { Observable, Subscription } from 'rxjs';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements AfterViewInit, OnDestroy {

  messages: MessageDecodedData[] = [];

  private chatId: number = 0;
  private chatEncoding: Int8Array | null = null;

  private infoMessageFlow: Subscription | null = null;
  private mainMessageFlow: Subscription | null = null;;

  constructor(private chatService: ChatService,
    private signalRService: SignalRService,
    private route: ActivatedRoute) { }

  async ngAfterViewInit(): Promise<void> {
    const _chatId = this.route.snapshot.paramMap.get('id') ?? '0';
    if (_chatId === '0') {
      console.log('wrong chat id')
      return;
    }

    this.chatId = +_chatId;

    const getChatInfo = await this.chatService.EnterChat(this.chatId);
    const secretKey = this.randomInteger(1, getChatInfo.prime);

    const chatSharedKey = this.fromByteArrayToBigIntegerJSBN(getChatInfo.chatPublicKey);
    const myPublicKey = new BigInteger.BigInteger(getChatInfo.primitiveRootModule.toString())
      .modPow(new BigInteger.BigInteger(secretKey.toString()),
        new BigInteger.BigInteger(getChatInfo.prime.toString()));

    const chatEncodingResponse = await this.chatService.GetChatEncoding(this.chatId,
      Int8Array.from(myPublicKey.toByteArray()));

    const sharedKey = chatSharedKey.modPow(new BigInteger.BigInteger(secretKey.toString()),
      new BigInteger.BigInteger(getChatInfo.prime.toString()));


    this.chatEncoding = chatEncodingResponse.encodedEncodingKey;
    if (!this.chatEncoding) {
      console.error('Error while getting chat encoding');
    }

    const key = CryptoJS.PBKDF2(sharedKey.toByteArray.toString(),
      "some salt message", { iterations: 10_000 });

    this.signalRService.startConnection().then(async () => {

      try {
        await this.signalRService.enterGroup(this.chatId, 'Temp user')
      } catch (err) {
        console.error(err);
        return;
      }

      console.log('connected to chat');

      this.signalRService.listenToAllMessages();

      this.mainMessageFlow = this.signalRService.messageFlow.subscribe(data => {
        console.log('messageFlow: ', data);

        const byteData = this.strToUtf16Bytes(data.encodedData);

        const decryptedData = CryptoJS.AES.decrypt(byteData.toString(),
          key).toString(CryptoJS.enc.Utf8);

        const decodedMessage: MessageDecodedData = JSON.parse(decryptedData);
        this.messages.push(decodedMessage);
      });

      this.infoMessageFlow = this.signalRService.infoMessageFlow.subscribe(data => {
        console.log(data);
      })

    })

  }

  strToUtf16Bytes(str: string) {
    const bytes = [];
    for (let ii = 0; ii < str.length; ii++) {
      const code = str.charCodeAt(ii); // x00-xFFFF
      bytes.push(code & 255, code >> 8); // low, high
    }
    return bytes;
  }

  private fromByteArrayToBigIntegerJSBN(byteArr: Int8Array): BigInteger.BigInteger {
    const zero = new BigInteger.BigInteger('0');
    const one = new BigInteger.BigInteger('1');
    const n256 = new BigInteger.BigInteger('256');

    let result = zero;
    let base = one;
    byteArr.forEach(byte => {
      result = result.add(base.multiply(new BigInteger.BigInteger(byte.toString())))
      base = base.multiply(n256);
    })

    return result;
  }

  private randomInteger(min: number, max: number) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
  }

  async ngOnDestroy(): Promise<void> {
    try {
      await this.signalRService.leaveGroup(this.chatId, 'Temp user')
    } catch (err) {
      console.error(err);
    }

    this.infoMessageFlow?.unsubscribe();
    this.mainMessageFlow?.unsubscribe();
  }
}
