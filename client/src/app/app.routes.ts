import { Routes } from '@angular/router';
import { Home } from './features/home/home';
import { Shop } from './features/shop/shop';
import { ProductDetails } from './features/shop/product-details/product-details';
import { TestError } from './features/test-error/test-error';
import { ServerError } from './shared/components/server-error/server-error';
import { NotFound } from './shared/components/not-found/not-found';
import { Cart } from './features/cart/cart';
import { Checkout } from './features/checkout/checkout';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'shop', component: Shop },
  { path: 'shop/:id', component: ProductDetails },
  { path: 'cart', component: Cart },
  { path: 'checkout', component: Checkout },
  { path: 'test-error', component: TestError },
  { path: 'server-error', component: ServerError },
  { path: 'not-found', component: NotFound },
  { path: '**', redirectTo: 'not-found', pathMatch: 'full' },
];
