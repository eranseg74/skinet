import { Component, inject } from '@angular/core';
import { CartService } from '../../core/services/cart-service';
import { CartItem } from './cart-item/cart-item';
import { OrderSummery } from '../../shared/components/order-summery/order-summery';
import { EmptyState } from '../../shared/components/empty-state/empty-state';

@Component({
  selector: 'app-cart',
  imports: [CartItem, OrderSummery, EmptyState],
  templateUrl: './cart.html',
  styleUrl: './cart.scss',
})
export class Cart {
  protected cartService = inject(CartService);
}
