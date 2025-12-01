import { computed, inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Cart } from '../../shared/models/cart';
import { CartItem } from '../../shared/models/cart';
import { Product } from '../../shared/models/product';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  baseUrl = environment.apiUrl; // Important! Import the production file and not the development file for later deployment. When working in development mode the files will be swapped according to what is defined in the angular.json file
  private http = inject(HttpClient);
  cart = signal<Cart | null>(null);
  // Using a computed signal to get the total items in the cart. A computed signal will re-evaluate its value when one of the signals that it depends on changes. In this case it depends on the cart signal
  itemCount = computed(() => {
    // If there is no cart return 0. The reduce function will iterate through all the items in the cart and sum their quantity property to get the total number of items in the cart. The initial value of the sum is 0
    return this.cart()?.items.reduce((sum, item) => sum + item.quantity, 0);
  });

  totals = computed(() => {
    const cart = this.cart(); // Could be null so we need to check
    if (!cart) return null;
    const subtotal = cart.items.reduce((sum, item) => sum + item.price * item.quantity, 0);
    const shipping = 0;
    const discount = 0;
    return {
      subtotal,
      shipping,
      discount,
      total: subtotal + shipping - discount,
    };
  });

  // Setting the cart signal in both cases - when getting a cart by id, and when creating a new cart
  // Subscribing to the observable and then returning will return an object of type Subscriber. Since we want to return an observable we will use the pipe() function
  getCart(id: string) {
    return this.http.get<Cart>(this.baseUrl + 'cart?id=' + id).pipe(
      map((cart) => {
        this.cart.set(cart);
        return cart;
      })
    );
  }

  setCart(cart: Cart) {
    return this.http.post<Cart>(this.baseUrl + 'cart', cart).subscribe({
      next: (cart) => this.cart.set(cart),
    });
  }

  // Since the method can get either a Product or a CartItem we will check and if it is a product we will map it to a cart item in order to work only on cart items
  addItemToCart(item: CartItem | Product, quantity = 1) {
    const cart = this.cart() ?? this.createCart();
    if (this.isProduct(item)) {
      item = this.mapProductToCartItem(item);
    }
    // Passing the items array, the item that we want to add or update (incase it already exists) and the quantity to add
    cart.items = this.addOrUpdateItem(cart.items, item, quantity);
    this.setCart(cart); // Setting the cart in the DB
  }

  private addOrUpdateItem(items: CartItem[], item: CartItem, quantity: number): CartItem[] {
    const index = items.findIndex((x) => x.productId === item.productId);
    if (index === -1) {
      item.quantity = quantity;
      items.push(item);
    } else {
      items[index].quantity += quantity;
    }
    return items;
  }

  removeItemFromCart(productId: number, quantity: number = 1) {
    const cart = this.cart();
    if (!cart) return; // exiting from the function without doing anything
    const index = cart.items.findIndex((x) => x.productId === productId);
    if (index !== -1) {
      if (cart.items[index].quantity > quantity) {
        cart.items[index].quantity -= quantity;
      } else {
        cart.items.splice(index, 1); // This will remove the product from the items list because if the given quantity is higher then the current quantity that means that there will be no more items from that product.
      }
      if (cart.items.length === 0) {
        // In case there are no more products in the cart
        this.deleteCart();
      } else {
        this.setCart(cart); // Updating the cart on the Redis server
      }
    }
  }

  // Delete the cart from the Redis server and removing the cart_id from the local storage
  deleteCart() {
    this.http.delete(this.baseUrl + 'cart?id=' + this.cart()?.id).subscribe({
      next: () => {
        localStorage.removeItem('cart_id');
        this.cart.set(null);
      },
    });
  }

  // private mapProductToCartItem(item: Product): CartItem | Product { // Removing the Product option because we will return only a CartItem
  private mapProductToCartItem(item: Product): CartItem {
    return {
      productId: item.id,
      productName: item.name,
      price: item.price,
      quantity: 0,
      pictureUrl: item.pictureUrl,
      brand: item.brand,
      type: item.type,
    };
  }

  // If the item has an id because the product has an id (and the cartItem does not. It has a productId) then this is a Product, then the phrase 'item is Product' will be evaluated to true and this is the value that will be returned
  private isProduct(item: CartItem | Product): item is Product {
    return (item as Product).id !== undefined;
  }

  // private createCart(): Cart | null { // Removing the null because we will always get a cart
  private createCart(): Cart {
    const cart = new Cart();
    localStorage.setItem('cart_id', cart.id);
    return cart;
  }
}
