import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbActiveModal, NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { GoogleMapsModule } from '@angular/google-maps';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LocationDetailComponent } from './components/location-detail/location-detail.component';
import { LoginComponent } from './components/auth/login/login.component';
import { RegisterComponent } from './components/auth/register/register.component';
import { AuthInterceptor } from './middleware/auth.interceptor';
import { SearchComponent } from './components/search/search.component';
import { FilterComponent } from './components/filter/filter.component';
import { FeedbackComponent } from './components/feedback/feedback.component';
import { AdminDashboardComponent } from './components/admin-dashboard/admin-dashboard.component';
import { NeedsManagementComponent } from './components/needs-management/needs-management.component';
import { DonationOfferingComponent } from './components/donation-offering/donation-offering.component';
import { DonationTrackingComponent } from './components/donation-tracking/donation-tracking.component';
import { PasswordRecoveryComponent } from './components/auth/password-recovery/password-recovery.component';
import { ResetPasswordComponent } from './components/auth/reset-password/reset-password.component';

@NgModule({
  declarations: [
    AppComponent,
    LocationDetailComponent,
    LoginComponent,
    RegisterComponent,
    FilterComponent,
    FeedbackComponent,
    NeedsManagementComponent,
    DonationOfferingComponent,
    DonationTrackingComponent,
    PasswordRecoveryComponent,
    ResetPasswordComponent,
    AdminDashboardComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    NgbModule,
    GoogleMapsModule
  ],
  providers: [
    NgbActiveModal,
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
