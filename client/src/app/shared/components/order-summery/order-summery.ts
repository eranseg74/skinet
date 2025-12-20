import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormField, MatLabel } from '@angular/material/select';
import { MatInput } from '@angular/material/input';
import { CartService } from '../../../core/services/cart-service';
import { CurrencyPipe, Location } from '@angular/common';
import { StripeService } from '../../../core/services/stripe-service';
import { firstValueFrom } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-order-summery',
  imports: [
    RouterLink,
    MatButtonModule,
    MatFormField,
    MatLabel,
    MatInput,
    CurrencyPipe,
    FormsModule,
    MatIconModule,
  ],
  templateUrl: './order-summery.html',
  styleUrl: './order-summery.scss',
})
export class OrderSummery {
  protected cartSrvice = inject(CartService);
  private stripeService = inject(StripeService);
  // The Location service can be used to interact with a browser's URL. To get data. To go to a different URL use the Router
  location = inject(Location); // Make sure the Location comes from @angular/common!!!
  code?: string;

  applyCouponCode(): void {
    if (!this.code) return;
    this.cartSrvice.applyDiscount(this.code).subscribe({
      next: async (coupon) => {
        const cart = this.cartSrvice.cart();
        if (cart) {
          cart.coupon = coupon;
          await firstValueFrom(this.cartSrvice.setCart(cart));
          this.code = undefined;
        }
        if (this.location.path() === '/checkout') {
          await firstValueFrom(this.stripeService.createOrUpdatePaymentIntent());
        }
      },
    });
  }

  async removeCouponCode() {
    const cart = this.cartSrvice.cart();
    if (!cart) return;
    if (cart.coupon) cart.coupon = undefined;
    await firstValueFrom(this.cartSrvice.setCart(cart));
    if (this.location.path() === '/checkout') {
      await firstValueFrom(this.stripeService.createOrUpdatePaymentIntent());
    }
  }
}
