import { CanActivateFn, Router } from '@angular/router';
import { CartService } from '../services/cart-service';
import { inject } from '@angular/core';
import { SnackbarService } from '../services/snackbar-service';
import { of } from 'rxjs';

export const emptyCartGuard: CanActivateFn = (route, state) => {
  const cartService = inject(CartService);
  const snack = inject(SnackbarService);
  const router = inject(Router);
  console.log(cartService.cart()?.items);
  if (!cartService.cart() || cartService.cart()?.items.length === 0) {
    snack.error('Your cart is empty');
    router.navigateByUrl('/cart');
    return false;
  }
  return true;
};
