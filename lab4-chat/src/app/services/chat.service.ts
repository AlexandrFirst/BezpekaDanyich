import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { UserChatInfo } from '../models/requests/IUserLogin';

@Injectable({
  providedIn: 'root'
})
export class ChatService {

  constructor(private httpClient: HttpClient) { }

  public async createChat(chatName: string, userId: number): Promise<UserChatInfo>{
    const creationResult = await this.httpClient.post(environment.baseApi + 'chat/create', {
      name: chatName,
      creatorId: userId
    }).toPromise()
    return creationResult as UserChatInfo;
  }
}
