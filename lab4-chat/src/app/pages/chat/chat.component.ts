import { AfterViewInit, Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MessageDecodedData, MessageEncodedData } from 'src/app/models/responses/IMessageData';
import { ChatService } from 'src/app/services/chat.service';
import { SignalRService } from 'src/app/services/signal-r.service';
import * as BigInteger from 'jsbn';
import * as CryptoJS from 'crypto-js';
import { Observable, Subscription } from 'rxjs';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.scss']
})
export class ChatComponent implements AfterViewInit, OnDestroy {

  messages: MessageDecodedData[] = [];

  private chatId: number = 0;
  private realChatEncoding: CryptoJS.lib.WordArray | string = "";

  private infoMessageFlow: Subscription | null = null;
  private mainMessageFlow: Subscription | null = null;;

  constructor(private chatService: ChatService,
    private signalRService: SignalRService,
    private route: ActivatedRoute,
    private authService: AuthService) { }

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


    const myPublicKeyArr = myPublicKey.toByteArray().reverse();
    const chatEncodingResponse = await this.chatService.GetChatEncoding(this.chatId,
      myPublicKeyArr);

    const secretKeyBI = new BigInteger.BigInteger(secretKey.toString());
    const primeBI = new BigInteger.BigInteger(getChatInfo.prime.toString());

    const sharedKey = chatSharedKey.modPow(secretKeyBI, primeBI);


    const chatEncoding = chatEncodingResponse.encodedEncodingKey;
    const realChatEncoding = chatEncodingResponse.notEncodedEncodingKey;


    debugger;
    if (!chatEncoding) {
      console.error('Error while getting chat encoding');
    }
    debugger;
    let ba_bits = sharedKey.toString(2);
    while (ba_bits.length % 8 !== 0) {
      ba_bits = '0' + ba_bits;
    }

    const bytes: number[] = [];
    for (let i = 0; i < ba_bits.length; i += 8) {
      const byte = parseInt(ba_bits.slice(i, i + 8), 2);
      bytes.push(byte);
    }

    const password = this.byteArrayToWordArray(bytes);

    const p = sharedKey.toString(16);
    const k = CryptoJS.enc.Hex.parse(p);

    const salt = CryptoJS.enc.Base64.parse("cwBkAHMAZABzAGQ"); // "some salt message

    const keyAndIv = CryptoJS.PBKDF2(k,
     salt, { iterations: 10_000, keySize: 4 + 2 });
    let hexKeyAndIv = CryptoJS.enc.Hex.stringify(keyAndIv);

    let key = CryptoJS.enc.Hex.parse(hexKeyAndIv.substring(0, 32));
    let iv = CryptoJS.enc.Hex.parse(hexKeyAndIv.substring(32, hexKeyAndIv.length));


    this.realChatEncoding = CryptoJS.AES.decrypt(chatEncoding, key,
      {
        iv: iv,
        mode: CryptoJS.mode.CBC,
        padding: CryptoJS.pad.Pkcs7
      });

      console.log(CryptoJS.enc.Base64.stringify(this.realChatEncoding))
    // console.log('toString():', this.realChatEncoding.toString());
    // console.log('Base64:', this.realChatEncoding.toString(CryptoJS.enc.Base64));
    // console.log('Hex:', this.realChatEncoding.toString(CryptoJS.enc.Hex));
    // console.log('Utf16:', this.realChatEncoding.toString(CryptoJS.enc.Utf16));
    // console.log('Utf16LE:', this.realChatEncoding.toString(CryptoJS.enc.Utf16LE));
    // console.log('Utf8:', this.realChatEncoding.toString(CryptoJS.enc.Utf8));

    this.signalRService.startConnection().then(async () => {

      console.log('connected to chat');

      this.signalRService.listenToAllMessages();

      this.mainMessageFlow = this.signalRService.messageFlow.subscribe(data => {
        debugger;

        console.log('messageFlow: ', data);

        const byteData = this.strToUtf16Bytes(data.encodedData);

        const decryptedData = CryptoJS.AES.decrypt(byteData.toString(),
          this.realChatEncoding).toString(CryptoJS.enc.Utf16LE);

        const decodedMessage: MessageDecodedData = JSON.parse(decryptedData);
        this.messages.push(decodedMessage);
      });

      this.infoMessageFlow = this.signalRService.infoMessageFlow.subscribe(data => {
        console.log('info flow', data);
      })

      try {
        await this.signalRService.enterGroup(this.chatId, 'Temp user')
      } catch (err) {
        console.error(err);
        return;
      }
    })
  }

  private toHexString(byteArray: number[]) {
    return Array.from(byteArray, function (byte) {
      return ('0' + (byte & 0xFF).toString(16)).slice(-2);
    }).join('')
  }

  public sendMessageBtnClick(element: any) {
    const message = element.value;
    this.sendMessage(message);
    element.value = '';
  }

  private sendMessage(rawMessage: string) {
    debugger;
    const messageToEncrypts: MessageDecodedData = {
      message: rawMessage,
      name: 'Temp',
      userId: this.authService.userId
    }

    const jsonFormat = JSON.stringify(messageToEncrypts);

    const salt = CryptoJS.enc.Base64.parse("cwBkAHMAZABzAGQ"); // "some salt message

    let keyAndIv = CryptoJS.PBKDF2(this.realChatEncoding,
      salt, { iterations: 10000, keySize: 6 });

    let hexKeyAndIv = CryptoJS.enc.Hex.stringify(keyAndIv);

    let key = CryptoJS.enc.Hex.parse(hexKeyAndIv.substring(0, 32));
    let iv = CryptoJS.enc.Hex.parse(hexKeyAndIv.substring(32, hexKeyAndIv.length));

    let encryptedStr = CryptoJS.AES.encrypt(CryptoJS.enc.Utf16LE.parse(jsonFormat), key, {
      iv: iv,
      mode: CryptoJS.mode.CBC,
      padding: CryptoJS.pad.Pkcs7
    }).toString();

    this.signalRService.sendMessage(this.chatId, encryptedStr).then(response => {
      console.log(`message to chat ${this.chatId} send`)
    })
  }


  private byteArrayToWordArray(ba: any): CryptoJS.lib.WordArray {
    var wa: any = [],
      i;
    for (i = 0; i < ba.length; i++) {
      wa[(i / 4) | 0] |= ba[i] << (24 - 8 * i);
    }

    return CryptoJS.lib.WordArray.create(wa, ba.length);
  }


  private strToUtf16Bytes(str: string) {
    const bytes = [];
    for (let ii = 0; ii < str.length; ii++) {
      const code = str.charCodeAt(ii); // x00-xFFFF
      bytes.push(code & 255, code >> 8); // low, high
    }
    return bytes;
  }

  private fromByteArrayToBigIntegerJSBN(byteArr: number[]): BigInteger.BigInteger {
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
