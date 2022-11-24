import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-auth-page',
  templateUrl: './auth-page.component.html',
  styleUrls: ['./auth-page.component.scss']
})
export class AuthPageComponent {

  loginForm = new FormGroup({
    userName: new FormControl('', Validators.required),
  })

  constructor(private authService: AuthService,
    private router: Router) { }

  async loginFormSubmit(data: any) {
    const authResponse = await this.authService.login(data.userName);
    if (authResponse) {
      this.router.navigate(['c'])
    }
  }

}
