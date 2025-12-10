import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCard } from '@angular/material/card';
import { MatInput } from '@angular/material/input';
import { MatError, MatFormField, MatLabel } from '@angular/material/select';
import { AccountService } from '../../../core/services/account-service';
import { Router } from '@angular/router';
import { SnackbarService } from '../../../core/services/snackbar-service';
import { JsonPipe } from '@angular/common';
import { TextInput } from '../../../shared/components/text-input/text-input';

@Component({
  selector: 'app-register',
  imports: [
    ReactiveFormsModule,
    MatCard,
    MatFormField,
    MatLabel,
    MatInput,
    MatButtonModule,
    JsonPipe,
    TextInput,
  ],
  templateUrl: './register.html',
  styleUrl: './register.scss',
})
export class Register {
  // Required services for the Register component
  private fb = inject(FormBuilder);
  private accountService = inject(AccountService);
  private router = inject(Router);
  private snack = inject(SnackbarService);
  validationErrors = signal<string[] | null>(null);

  // Form Group and its Form Controls
  registerForm = this.fb.group({
    // Validation on the client side in reactive forms is done by adding validators to the form controls as the second argument. For more then 1 validator we need to put them in an array
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    /*
    The regular expression (?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[\(@\)!%*?&])[A-Za-z\\d$@$!%*?&].{8,} enforces the following password requirements:
    - (?=.*[a-z]): Asserts that there is at least one lowercase letter.
    - (?=.*[A-Z]): Asserts that there is at least one uppercase letter
    - (?=.*[0-9]): Asserts that there is at least one digit.
    - (?=.*[\(@\)!%*?&]): Asserts that there is at least one special character from the set $ @ $ ! % * ? &. 
    - [A-Za-z\\d$@$!%*?&]: Matches any allowed character (alphanumeric and the specified special characters).
    - .{8,}: Ensures the password is at least 8 characters long.
    */
    password: [
      '',
      [
        Validators.required,
        Validators.pattern(
          '(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])(?=.*[$@$!%*?&])[A-Za-z\\d$@$!%*?&].{7,}'
        ),
      ],
    ],
  });

  // Methods
  onSubmit() {
    this,
      this.accountService.register(this.registerForm.value).subscribe({
        next: () => {
          this.snack.success('Registration successful - you can now login');
          this.router.navigateByUrl('/account/login');
        },
        error: (errors) => this.validationErrors.set(errors),
      });
  }
}
