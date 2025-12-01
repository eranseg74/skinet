import { Component, inject } from '@angular/core';
import { CartService } from '../../core/services/cart-service';
import { CartItem } from './cart-item/cart-item';
import { OrderSummery } from '../../shared/components/order-summery/order-summery';

@Component({
  selector: 'app-cart',
  imports: [CartItem, OrderSummery],
  templateUrl: './cart.html',
  styleUrl: './cart.scss',
})
export class Cart {
  protected cartService = inject(CartService);
}
