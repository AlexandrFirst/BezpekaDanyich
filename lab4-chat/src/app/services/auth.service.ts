import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { first, firstValueFrom, Observable, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IUserLoginRquest, IUserLoginResponse, UserChatInfo } from '../models/requests/IUserLogin';


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private isUserLoggedIn: boolean = false;

  private userInfo: IUserLoginResponse | null = null;

  constructor(private httpClient: HttpClient) {
    const isUserLoggedIn = localStorage.getItem('isUserLoggedIn');
    if (isUserLoggedIn) {
      this.isUserLoggedIn = true;
    }
  }

  public async login(name: string): Promise<boolean> {

    try {
      const loginRequest = await firstValueFrom(this.httpClient.post(environment.baseApi + 'user/login', {
        userName: name
      } as IUserLoginRquest));

      this.userInfo = loginRequest as IUserLoginResponse

      localStorage.setItem('isUserLoggedIn', 'true');

      this.isUserLoggedIn = true;

      return true;

    } catch (err) {
      console.error(err);
      return false;
    }
  }

  public logout(){
    localStorage.removeItem('isUserLoggedIn');
    this.isUserLoggedIn = false;
  }

  public get isLoggedIn(): boolean {
    return this.isUserLoggedIn;
  }

  public get allUserChats(): UserChatInfo[] | undefined {
    return this.userInfo?.userChatInfos
  }

  public set updateUserChats(newUserChats: UserChatInfo[]){
    this.userInfo!.userChatInfos = [];
    this.userInfo!.userChatInfos.push(...newUserChats);
  }
}
