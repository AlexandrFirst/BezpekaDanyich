import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Route, RouterModule, Routes } from '@angular/router';
import { ChatListComponent } from 'src/app/pages/chat-list/chat-list.component';
import { ChatComponent } from 'src/app/pages/chat/chat.component';

const routes: Routes = [
  {path: 'list', component: ChatListComponent},
  {path: 'chat/:id', component: ChatComponent},
]

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports:[
    RouterModule
  ]
})
export class MainContentRoutingModule { }
