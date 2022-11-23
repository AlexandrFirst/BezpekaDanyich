import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChatListComponent } from 'src/app/pages/chat-list/chat-list.component';
import { ChatComponent } from 'src/app/pages/chat/chat.component';
import { HttpClientModule } from '@angular/common/http';
import { MainContentRoutingModule } from '../main-content-routing/main-content-routing.module';


@NgModule({
  declarations: [
    ChatListComponent,
    ChatComponent
  ],
  imports: [
    CommonModule,
    HttpClientModule,
    MainContentRoutingModule 
  ]
})
export class MainContentModule { }
