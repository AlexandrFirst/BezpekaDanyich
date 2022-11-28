import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom, map, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { ChatEncodingRequest, ChatInfoRequest } from '../models/requests/IChatRequest';
import { UserChatInfo } from '../models/requests/IUserLogin';
import { ChatEncodingResponse, ChatInfoResponse } from '../models/responses/IChatResponse';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  constructor(private httpClient: HttpClient, private authService: AuthService) { }

  public async createChat(chatName: string, userId: number): Promise<UserChatInfo> {
    const creationResult = await this.httpClient.post(environment.baseApi + 'chat/create', {
      name: chatName,
      creatorId: userId
    }).toPromise()
    return creationResult as UserChatInfo;
  }

  public async EnterChat(chatId: number): Promise<ChatInfoResponse> {
    const requestParams = {
      chatId: chatId,
      userId: this.authService.userId
    } as ChatInfoRequest;

    const chatResponseInfo = await firstValueFrom(this.httpClient.post(environment.baseApi + 'chat/info', requestParams)
      .pipe(map(x => {
        console.log(x);
        return x as ChatInfoResponse
      })));

   // chatResponseInfo.chatPublicKey = this.stringToByteArray(chatResponseInfo.chatPublicKey as unknown as string)

    return chatResponseInfo;
  }


  private stringToByteArray(s: string): number[] {

    var result: number[] = [];
    for (var i = 0; i < s.length; i++) {
      result[i] = s.charCodeAt(i);
    }
    return result;
  }

  public async GetChatEncoding(chatId: number, clientsKey: number[]) {
    const requestParams = {
      chatId: chatId,
      userId: this.authService.userId,
      clientsKey: clientsKey
    } as ChatEncodingRequest;

    const chatResponseInfo = await firstValueFrom(this.httpClient.post(environment.baseApi + 'chat/chatEncoding', requestParams)
      .pipe(map(x => x as ChatEncodingResponse)));

    return chatResponseInfo;
  }
}
