import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCard } from '@angular/material/card';
import { MatInput } from '@angular/material/input';
import { MatFormField, MatLabel } from '@angular/material/select';
import { AccountService } from '../../../core/services/account-service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule, MatCard, MatFormField, MatInput, MatLabel, MatButtonModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  // The most coomon way to use reactive forms is to use the FormBuilder service provided by AngularForms
  private fb = inject(FormBuilder);
  private accountService = inject(AccountService);
  private router = inject(Router);
  private activatedRoute = inject(ActivatedRoute);
  returnUrl = '/shop';

  constructor() {
    const url = this.activatedRoute.snapshot.queryParams['returnUrl'];
    if (url) this.returnUrl = url;
  }

  loginForm = this.fb.group({
    email: [''],
    password: [''],
  });

  onSubmit() {
    this.accountService.login(this.loginForm.value).subscribe({
      next: () => {
        this.accountService.getUserInfo().subscribe(); // Getting the user object that contains all of the user's details. We have to subscribe to the getUserInfo method here because we are not subscribing to it in the accountService and if we are not subscr. We have to subscribe here to the getUserInfo method because this method is responsible to set the current user
        this.router.navigateByUrl(this.returnUrl);
      },
    });
  }
}
