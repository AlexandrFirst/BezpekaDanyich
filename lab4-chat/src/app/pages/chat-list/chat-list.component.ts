import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { AddChatComponent } from 'src/app/components/add-chat/add-chat.component';
import { UserChatInfo } from 'src/app/models/requests/IUserLogin';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-chat-list',
  templateUrl: './chat-list.component.html',
  styleUrls: ['./chat-list.component.scss']
})
export class ChatListComponent {

  private m_userChats: UserChatInfo[] = [];

  constructor(private authService: AuthService, private dialogService: MatDialog) {
    authService.InitingObservable.subscribe(val => {
      this.m_userChats = val.userChatInfos
    })
  }

  public addChat() {
    debugger;
    if (this.dialogService.openDialogs.length < 1) {
      this.dialogService.open(AddChatComponent, {
        disableClose: false,
        height: '100px',
        width: '400px'
      })
    }
  }

  get userChats(): UserChatInfo[] {
    return this.m_userChats;
  }
}
