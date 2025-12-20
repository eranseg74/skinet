import { Component, inject, OnInit, output } from '@angular/core';
import { CheckoutService } from '../../../core/services/checkout-service';
import { MatRadioModule } from '@angular/material/radio';
import { CurrencyPipe } from '@angular/common';
import { CartService } from '../../../core/services/cart-service';
import { DeliveryMethod } from '../../../shared/models/deliveryMethod';
import { firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-checkout-delivery',
  imports: [MatRadioModule, CurrencyPipe],
  templateUrl: './checkout-delivery.html',
  styleUrl: './checkout-delivery.scss',
})
export class CheckoutDelivery implements OnInit {
  protected checkoutService = inject(CheckoutService);
  protected cartService = inject(CartService);
  deliveryComplete = output<boolean>(); // Not initializing it with anything

  ngOnInit(): void {
    // When loading the component (this will be displayed in step 2 of the checkout process) we first get all the delivery methods from the checkoutService. Then, if the cart has a delivery method (the user chose a delivery method from the radio buttons) the cart holds only the delivery method id and we need all the data in order to be able to compute the total cost of the order, including the delivery cost so we find the appropriate delivery method from the delivery methods array according to the delivery method id in the cart. Then we set the selectedDelivery signal in the cartService to the found delivery method
    this.checkoutService.getDeliveryMethods().subscribe({
      next: (methods) => {
        if (this.cartService.cart()?.deliveryMethodId) {
          const method = methods.find((x) => x.id === this.cartService.cart()?.deliveryMethodId);
          if (method) {
            this.cartService.selectedDelivery.set(method);
            // If there is a method we can presume thata the user already selected a delivery method so we want to emit the deliveryComplete property as true, to the parent element (the checkout component) so there we will mark the delivery step as completed. For that we defined the deliveryComplete field here as an output. Now we can emit it:
            this.deliveryComplete.emit(true);
          }
        }
      },
    }); // Subscribing here so we will populate the delivery methods inside the service
  }

  async updateDeliveryMethod(method: DeliveryMethod) {
    this.cartService.selectedDelivery.set(method);
    const cart = this.cartService.cart();
    if (cart) {
      cart.deliveryMethodId = method.id;
      await firstValueFrom(this.cartService.setCart(cart)); // This will also update our Redis DB
      this.deliveryComplete.emit(true); // Also emitting the deliveryComplete as true on each delivery method update
    }
  }
}
