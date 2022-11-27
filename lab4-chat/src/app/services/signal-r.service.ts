import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Subject } from 'rxjs';
import { MessageEncodedData } from '../models/responses/IMessageData';

@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  private hubConnection: any;

  public messageFlow: Subject<MessageEncodedData> = new Subject<MessageEncodedData>();
  public infoMessageFlow: Subject<string> = new Subject<string>();

  constructor() { }

  public startConnection() {
    return new Promise((resolve, reject) => {
      this.hubConnection = new HubConnectionBuilder()
        .withUrl("https://localhost:5000/chat").build();

      this.hubConnection.start()
        .then(() => {
          console.log("connection established");
          return resolve(true);
        })
        .catch((err: any) => {
          console.log("error occured" + err);
          reject(err);
        });
    });
  }

  public listenToAllMessages() {
    (<HubConnection>this.hubConnection).on("SendMessageToChat", (data) => {
      console.log("SendMessageToChat: ", data);

      this.messageFlow.next({
        encodedData: data
      } as MessageEncodedData)
    });

    (<HubConnection>this.hubConnection).on("EnterGroup", (userName) => {
      console.log("EnterGroup: ", userName);

      this.infoMessageFlow.next(`User ${userName} entered chat`);
    });

    (<HubConnection>this.hubConnection).on("LeaveGroup", (userName) => {
      console.log("LeaveGroup: ", userName);

      this.infoMessageFlow.next(`User ${userName} left chat`);
    })
  }

  public enterGroup(chatId: number, userName: string) {
    return new Promise((res, rej) => {
      (<HubConnection>this.hubConnection)
        .invoke("EnterGroup", chatId, userName)
        .then(() => {
          console.log(`User ${userName} entered chat with id: ${chatId}`)
          return res(true);
        }, (err) => {
          console.error(err);
          rej(false);
        })
    })
  }

  public leaveGroup(chatId: number, userName: string) {
    return new Promise((res, rej) => {
      (<HubConnection>this.hubConnection)
        .invoke("LeaveGroup", chatId, userName)
        .then(() => {
          console.log(`User ${userName} entered chat with id: ${chatId}`)
          return res(true);
        }, (err) => {
          console.error(err);
          rej(false);
        })
    })
  }
}
