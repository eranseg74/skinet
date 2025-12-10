import { inject, Injectable } from '@angular/core';
import { CartService } from './cart-service';
import { forkJoin, of } from 'rxjs';
import { AccountService } from './account-service';

@Injectable({
  providedIn: 'root',
})
export class InitService {
  private cartService = inject(CartService);
  private accountService = inject(AccountService);

  init() {
    const cartId = localStorage.getItem('cart_id');
    // of(null) returns an observable of null
    const cart$ = cartId ? this.cartService.getCart(cartId) : of(null);
    // The forkJoin functions alllows multiple observables to complete and send their results in an array
    return forkJoin({
      cart: cart$,
      user: this.accountService.getUserInfo(),
    });
  }
}
