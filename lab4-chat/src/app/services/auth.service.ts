import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { first, firstValueFrom, map, Observable, Observer, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IUserLoginRquest, IUserLoginResponse, UserChatInfo } from '../models/requests/IUserLogin';


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private isUserLoggedIn: boolean = false;

  private userInfo!: IUserLoginResponse;

  public InitingObservable: Observable<IUserLoginResponse>;

  constructor(private httpClient: HttpClient) {
    const isUserLoggedIn = localStorage.getItem('isUserLoggedIn');
    const userId = localStorage.getItem('userId');

    if (isUserLoggedIn) {
      this.isUserLoggedIn = true;
    }

    this.InitingObservable = new Observable(observer => {
      if (userId) {
        this.getUserById(+userId).subscribe({
          next: (val) => {
            this.userInfo = val;
            observer.next(val)
          },
          error: (err) => {
            console.log(err)
          }
        })
      }
    })
  }

  private getUserById(userId: number): Observable<IUserLoginResponse> {
    return this.httpClient.get(environment.baseApi + `user/${userId}`).pipe(map(x => x as IUserLoginResponse));
  }

  public async login(name: string): Promise<boolean> {

    try {
      const loginRequest = await firstValueFrom(this.httpClient.post(environment.baseApi + 'user/login', {
        userName: name
      } as IUserLoginRquest));

      this.userInfo = loginRequest as IUserLoginResponse

      localStorage.setItem('isUserLoggedIn', 'true');
      localStorage.setItem('userId', this.userInfo.userId.toString());

      this.isUserLoggedIn = true;

      return true;

    } catch (err) {
      console.error(err);
      return false;
    }
  }

  public logout() {
    localStorage.removeItem('isUserLoggedIn');
    localStorage.removeItem('userId');
    this.isUserLoggedIn = false;
  }

  public get isLoggedIn(): boolean {
    return this.isUserLoggedIn;
  }

  public get allUserChats(): UserChatInfo[] {
    return this.userInfo.userChatInfos
  }

  public set updateUserChats(newUserChats: UserChatInfo[]) {
    this.userInfo!.userChatInfos = [];
    this.userInfo!.userChatInfos.push(...newUserChats);
  }
}
