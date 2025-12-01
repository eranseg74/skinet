import { Component, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { CartItem as CartItemModel } from '../../../shared/models/cart';
import { CurrencyPipe } from '@angular/common';
import { CartService } from '../../../core/services/cart-service';

@Component({
  selector: 'app-cart-item',
  imports: [RouterLink, MatButtonModule, MatIcon, CurrencyPipe],
  templateUrl: './cart-item.html',
  styleUrl: './cart-item.scss',
})
export class CartItem {
  // The input property for the cart item. The input is required. The input is used to pass data into the component. It returns the cart item data.
  item = input.required<CartItemModel>();
  cartService = inject(CartService);

  incrementQuantity() {
    this.cartService.addItemToCart(this.item());
  }

  decrementQuantity() {
    this.cartService.removeItemFromCart(this.item().productId);
  }

  // This will set the amount to 0 and according to the delete function logic, remove the entire item, and if it is the only item, delete the cart
  removeItemFromCart() {
    this.cartService.removeItemFromCart(this.item().productId, this.item().quantity);
  }
}
