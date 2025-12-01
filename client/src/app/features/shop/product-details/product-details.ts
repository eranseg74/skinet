import { Component, inject, OnInit, signal } from '@angular/core';
import { ShopService } from '../../../core/services/shop-service';
import { ActivatedRoute } from '@angular/router';
import { Product } from '../../../shared/models/product';
import { CurrencyPipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormField, MatLabel } from '@angular/material/select';
import { MatInput } from '@angular/material/input';
import { MatDividerModule } from '@angular/material/divider';
import { CartService } from '../../../core/services/cart-service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-product-details',
  imports: [
    CurrencyPipe,
    MatButtonModule,
    MatIconModule,
    MatFormField,
    MatInput,
    MatLabel,
    MatDividerModule,
    FormsModule,
  ],
  templateUrl: './product-details.html',
  styleUrl: './product-details.scss',
})
export class ProductDetails implements OnInit {
  private shopService = inject(ShopService);
  private activatedRoute = inject(ActivatedRoute);
  private cartService = inject(CartService);
  product = signal<Product | null>(null);
  quantityInCart = 0;
  quantity = 1;

  ngOnInit(): void {
    this.loadProduct();
  }

  loadProduct() {
    // We can get the snapshot of the active route and query it to get what we want from the request properties. In this case we get the route parameters which includes the id (as defined in the API call - shop/:id). The name must match the one specified in the appropriate path in the app.routes.ts file. We can get a null so we must check it. Also, everything that comes from the http call, comes as a string so we need to cast it to a number. We do so by adding the '+' sign before the variable.
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (!id) {
      return;
    }
    this.shopService.getProduct(+id).subscribe({
      next: (product) => {
        this.product.set(product);
        this.updateQuantityInCart();
      },
      error: (error) => console.log(error),
    });
  }

  updateCart() {
    // This is only for typescript benefit because we set the product as optional and the only reason for that is because we don't have it when the component is constructing
    const product = this.product();
    if (!product) return;
    if (this.quantity > this.quantityInCart) {
      const itmesToAdd = this.quantity - this.quantityInCart;
      this.quantityInCart += itmesToAdd;
      this.cartService.addItemToCart(product, itmesToAdd);
    } else {
      const itemsToRemove = this.quantityInCart - this.quantity;
      this.quantityInCart -= itemsToRemove;
      this.cartService.removeItemFromCart(product.id, itemsToRemove);
    }
  }

  updateQuantityInCart() {
    this.quantityInCart =
      this.cartService.cart()?.items.find((x) => x.productId === this.product()?.id)?.quantity || 0;
    this.quantity = this.quantityInCart || 1;
  }

  getButtonText() {
    return this.quantityInCart > 0 ? 'Update Quantity' : 'Add to Cart';
  }
}
