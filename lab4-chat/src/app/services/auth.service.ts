import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { first, firstValueFrom, Observable, of } from 'rxjs';
import { environment } from 'src/environments/environment';
import { IUserLoginRquest } from '../models/requests/IUserLogin';


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private isUserLoggedIn: boolean = false;

  constructor(private httpClient: HttpClient) { }

  async login(name: string): Promise<boolean> {

    const loginRequest = await firstValueFrom(this.httpClient.post(environment.baseApi + 'user/login', {
      name: name
    } as IUserLoginRquest));

    console.log(loginRequest)

    return true;
  }
}
