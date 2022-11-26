import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { HttpClientModule } from '@angular/common/http';

import { ReactiveFormsModule } from '@angular/forms';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthPageComponent } from './pages/auth-page/auth-page.component';
import { ChatListComponent } from './pages/chat-list/chat-list.component';
import { ChatComponent } from './pages/chat/chat.component';
import { MainContentModule } from './modules/main-content/main-content.module';
import { MainContentComponent } from './modules/main-content/main-content.component';
import {MatDialogModule} from '@angular/material/dialog';
import { AddChatComponent } from './components/add-chat/add-chat.component';
import { RemoveChatComponent } from './components/remove-chat/remove-chat.component';

@NgModule({
  declarations: [
    AppComponent,
    MainContentComponent,
    AuthPageComponent,
    AddChatComponent,
    RemoveChatComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    MainContentModule,
    ReactiveFormsModule,
    MatDialogModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
