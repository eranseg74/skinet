import { Routes } from '@angular/router';
import { Home } from './features/home/home';
import { Shop } from './features/shop/shop';
import { ProductDetails } from './features/shop/product-details/product-details';
import { TestError } from './features/test-error/test-error';
import { ServerError } from './shared/components/server-error/server-error';
import { NotFound } from './shared/components/not-found/not-found';
import { Cart } from './features/cart/cart';
import { Login } from './features/account/login/login';
import { Register } from './features/account/register/register';
import { authGuard } from './core/guards/auth-guard';
import { OrderComponent } from './features/orders/order';
import { OrderDetailed } from './features/orders/order-detailed/order-detailed';
import { Admin } from './features/admin/admin';
import { adminGuard } from './core/guards/admin-guard';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'shop', component: Shop },
  { path: 'shop/:id', component: ProductDetails },
  { path: 'cart', component: Cart },
  {
    // This code is the implementation of the Lazy loading. The checkout components will not load until the checkout is visited
    path: 'checkout',
    loadChildren: () =>
      import('./features/checkout/routes').then((routes) => routes.checkoutRoutes),
  },
  {
    path: 'orders',
    loadChildren: () => import('./features/orders/routes').then((routes) => routes.orderRoutes),
  },
  {
    path: 'account',
    loadChildren: () => import('./features/account/routes').then((routes) => routes.accoutRoutes),
  },
  { path: 'test-error', component: TestError },
  { path: 'server-error', component: ServerError },
  { path: 'not-found', component: NotFound },
  // In case of a single route that we want to use the lazy loading on we do not have to create a routes.ts file but d oit directly here:
  {
    path: 'admin',
    loadComponent: () => import('./features/admin/admin').then((component) => component.Admin),
    canActivate: [authGuard, adminGuard],
  },
  { path: '**', redirectTo: 'not-found', pathMatch: 'full' },
];
