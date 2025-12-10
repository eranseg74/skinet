import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Address, User } from '../../shared/models/user';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  currentUser = signal<User | null>(null);

  login(values: any) {
    let params = new HttpParams();
    params = params.append('useCookies', true);
    // We are not defining the URL as account/login because we are using the login method provided by the IdentityFramework. This is alsowhy it is not implemented in the AccountController in the API and this is why we should write 'login' and not 'account/login'.
    // We need to use the withCredentials because this is an authenticated EndPoint. It is not required here because it is executed using the AuthInterceptor. Note that the AuthInterceptor will send all the requests with a cookie, even those that does not need it.
    // return this.http.post<User>(this.baseUrl + 'login', values, { params, withCredentials: true });
    return this.http.post<User>(this.baseUrl + 'login', values, { params });
  }

  register(values: any) {
    return this.http.post(this.baseUrl + 'account/register', values);
  }

  getUserInfo() {
    // We need to use the withCredentials because this is an authenticated EndPoint. It is not required here because it is executed using the AuthInterceptor
    return (
      this.http
        // .get<User>(this.baseUrl + 'account/user-info', { withCredentials: true })
        .get<User>(this.baseUrl + 'account/user-info')
        // We cannot use the subscribe functionality here because we want to get from this call the user and not an observable (used in the init-service). This is why we use the pipe() functionality
        // .subscribe({
        //   next: (user) => this.currentUser.set(user),
        // Note!! that since we are not subscribing to this method here, we will need to subscribe to it somewhere else in order to track changes in the user. We do it in the login component
        .pipe(
          map((user) => {
            this.currentUser.set(user);
            return user; // Have to return a value because we are using the map RXJS function
          })
        )
    );
  }

  logout() {
    // We need to use the withCredentials because this is an authenticated EndPoint. It is not required here because it is executed using the AuthInterceptor
    // return this.http.post(this.baseUrl + 'account/logout', {}, { withCredentials: true });
    return this.http.post(this.baseUrl + 'account/logout', {});
  }

  updateAddress(address: Address) {
    return this.http.post(this.baseUrl + 'account/address', address);
  }

  getAuthState() {
    // Creating the type in the call itself using object notation
    return this.http.get<{ isAuthenticated: boolean }>(this.baseUrl + 'account/auth-status');
  }
}
