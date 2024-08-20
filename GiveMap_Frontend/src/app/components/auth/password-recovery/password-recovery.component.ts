import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-password-recovery',
  templateUrl: './password-recovery.component.html',
  styleUrls: ['./password-recovery.component.scss']
})
export class PasswordRecoveryComponent implements OnInit {
  recoveryForm: FormGroup;
  errorMessage: string = '';
  successMessage: string = '';

  constructor(
    private formBuilder: FormBuilder,
    private authService: AuthService
  ) {
    this.recoveryForm = this.formBuilder.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  ngOnInit(): void {}

  onSubmit(): void {
    if (this.recoveryForm.valid) {
      const { email } = this.recoveryForm.value;
      this.authService.requestPasswordReset(email).subscribe(
        (response) => {
          this.successMessage = 'Password reset instructions have been sent to your email.';
          this.errorMessage = '';
        },
        (error) => {
          this.errorMessage = error.error.message || 'An error occurred. Please try again.';
          this.successMessage = '';
        }
      );
    }
  }
}
