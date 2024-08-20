import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { MapComponent } from './components/map/map.component';
import { LocationDetailComponent } from './components/location-detail/location-detail.component';
import { UserProfileComponent } from './components/user-profile/user-profile.component';
import { LoginComponent } from './components/auth/login/login.component';
import { RegisterComponent } from './components/auth/register/register.component';
import { PasswordRecoveryComponent } from './components/auth/password-recovery/password-recovery.component';
import { ResetPasswordComponent } from './components/auth/reset-password/reset-password.component';
import { AdminDashboardComponent } from './components/admin-dashboard/admin-dashboard.component';
import { DonationTrackingComponent } from './components/donation-tracking/donation-tracking.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'location/:id', component: LocationDetailComponent },
  { path: 'profile', component: UserProfileComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'password-recovery', component: PasswordRecoveryComponent },
  { path: 'reset-password', component: ResetPasswordComponent },
  { path: 'admin', component: AdminDashboardComponent },
  { path: 'my-donations', component: DonationTrackingComponent },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
