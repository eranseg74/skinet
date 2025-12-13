import { Routes } from '@angular/router';
import { Home } from './features/home/home';
import { Shop } from './features/shop/shop';
import { ProductDetails } from './features/shop/product-details/product-details';
import { TestError } from './features/test-error/test-error';
import { ServerError } from './shared/components/server-error/server-error';
import { NotFound } from './shared/components/not-found/not-found';
import { Cart } from './features/cart/cart';
import { Checkout } from './features/checkout/checkout';
import { Login } from './features/account/login/login';
import { Register } from './features/account/register/register';
import { authGuard } from './core/guards/auth-guard';
import { emptyCartGuard } from './core/guards/empty-cart-guard';
import { CheckoutSuccess } from './features/checkout/checkout-success/checkout-success';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'shop', component: Shop },
  { path: 'shop/:id', component: ProductDetails },
  { path: 'cart', component: Cart },
  { path: 'checkout', component: Checkout, canActivate: [authGuard, emptyCartGuard] },
  // When the user is redirected to the success page after a successful checkout we want to make sure that only authenticated users can access this page so we are adding the authGuard here as well.
  { path: 'checkout/success', component: CheckoutSuccess, canActivate: [authGuard] },
  { path: 'account/login', component: Login },
  { path: 'account/register', component: Register },
  { path: 'test-error', component: TestError },
  { path: 'server-error', component: ServerError },
  { path: 'not-found', component: NotFound },
  { path: '**', redirectTo: 'not-found', pathMatch: 'full' },
];
