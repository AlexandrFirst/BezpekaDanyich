import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginGuard } from './guards/login.guard';
import { MainContentGuard } from './guards/main-content.guard';
import { MainContentComponent } from './modules/main-content/main-content.component';
import { AuthPageComponent } from './pages/auth-page/auth-page.component';

const routes: Routes = [
  { path: 'login', component: AuthPageComponent, canActivate: [LoginGuard] },
  {
    path: 'c', component: MainContentComponent, canActivate: [MainContentGuard],
    loadChildren: () => import('./modules/main-content/main-content.module').then(m => m.MainContentModule)
  },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
