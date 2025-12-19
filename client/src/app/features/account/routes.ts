import { Route } from '@angular/router';
import { Login } from './login/login';
import { Register } from './register/register';

export const accoutRoutes: Route[] = [
  { path: 'login', component: Login },
  { path: 'register', component: Register },
];
