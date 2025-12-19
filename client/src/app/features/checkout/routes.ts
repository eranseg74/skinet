import { Route } from '@angular/router';
import { authGuard } from '../../core/guards/auth-guard';
import { emptyCartGuard } from '../../core/guards/empty-cart-guard';
import { orderCompleteGuard } from '../../core/guards/order-complete-guard';
import { Checkout } from './checkout';
import { CheckoutSuccess } from './checkout-success/checkout-success';

export const checkoutRoutes: Route[] = [
  // These routes will be children of a main route specified in the app.routes.ts file
  { path: '', component: Checkout, canActivate: [authGuard, emptyCartGuard] },
  // When the user is redirected to the success page after a successful checkout we want to make sure that only authenticated users can access this page so we are adding the authGuard here as well.
  {
    path: 'success',
    component: CheckoutSuccess,
    canActivate: [authGuard, orderCompleteGuard],
  },
];
