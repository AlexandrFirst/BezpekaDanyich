import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthPageComponent } from './pages/auth-page/auth-page.component';
import { ChatListComponent } from './pages/chat-list/chat-list.component';
import { ChatComponent } from './pages/chat/chat.component';
import { MainContentModule } from './modules/main-content/main-content.module';
import { MainContentComponent } from './modules/main-content/main-content.component';

@NgModule({
  declarations: [
    AppComponent,
    MainContentComponent,
    AuthPageComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    MainContentModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
