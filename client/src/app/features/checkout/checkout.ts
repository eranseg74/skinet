import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { OrderSummery } from '../../shared/components/order-summery/order-summery';
import { MatStepper, MatStepperModule } from '@angular/material/stepper';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { StripeService } from '../../core/services/stripe-service';
import { SnackbarService } from '../../core/services/snackbar-service';
import {
  ConfirmationToken,
  StripeAddressElement,
  StripeAddressElementChangeEvent,
  StripePaymentElement,
  StripePaymentElementChangeEvent,
} from '@stripe/stripe-js';
import { MatCheckboxChange, MatCheckboxModule } from '@angular/material/checkbox';
import { StepperSelectionEvent } from '@angular/cdk/stepper';
import { Address } from '../../shared/models/user';
import { firstValueFrom } from 'rxjs';
import { AccountService } from '../../core/services/account-service';
import { CheckoutDelivery } from './checkout-delivery/checkout-delivery';
import { CheckoutReview } from './checkout-review/checkout-review';
import { CartService } from '../../core/services/cart-service';
import { CurrencyPipe, JsonPipe } from '@angular/common';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-checkout',
  imports: [
    OrderSummery,
    MatStepperModule,
    RouterLink,
    MatButtonModule,
    MatCheckboxModule,
    CheckoutDelivery,
    CheckoutReview,
    CurrencyPipe,
    JsonPipe,
    MatProgressSpinnerModule,
  ],
  templateUrl: './checkout.html',
  styleUrl: './checkout.scss',
})
export class Checkout implements OnInit, OnDestroy {
  private stripeService = inject(StripeService);
  private snackbar = inject(SnackbarService);
  private router = inject(Router);
  private accounbtService = inject(AccountService);
  protected cartService = inject(CartService);
  addressElement?: StripeAddressElement;
  paymentElement?: StripePaymentElement;
  saveAddress: boolean = false;
  // The completionStatus property is used to control the payment flow. Once a step is completed it will be defined as completed so we will know what steps are completed and block the user from moving to next steps if the current step is not completed.
  // completionStatus = signal<{ address: boolean; card: boolean; delivery: boolean }>({
  //   address: false,
  //   card: false,
  //   delivery: false,
  // });
  // This approach works! These parameters aere updated immediately
  protected addressComplete = signal(false);
  protected paymentComplete = signal(false);
  protected deliveryComplete = signal(false);

  confirmationToken = signal<ConfirmationToken | undefined>(undefined);

  loading = signal(false);

  // An example of how to bind a method in the class to the class itself:
  /*
  constructor() {
    this.handleAddressChange = this.handleAddressChange.bind(this);
  }
  */

  async ngOnInit() {
    try {
      // Here we are getting the address element from stripe which takes care of the address form
      this.addressElement = await this.stripeService.createAddressElement();
      // Here we are replacing the div with the address-element id, with the element we got from stripe
      this.addressElement.mount('#address-element');

      // Ssame for the payment element
      this.paymentElement = await this.stripeService.createPaymentElement();
      this.paymentElement.mount('#payment-element');
      // The 'on' function allows us to define w behavior for a certain event on the stripe elements. Here, the change event is triggered when the Element's value changes
      this.addressElement.on('change', this.handleAddressChange);
      this.paymentElement.on('change', this.handlePaymentChange);
    } catch (error: any) {
      this.snackbar.error(error.message);
    }
  }
  handlePaymentChange = (event: StripePaymentElementChangeEvent) => {
    // this.completionStatus.update((state) => {
    //   state.card = event.complete;
    //   return state;
    // });
    this.paymentComplete.set(event.complete);
  };

  // When we use a method the way we use it here (defining it on an event), this method must be bound to the class. We can bind it through the constructor (see how in the constructor in the comments above) or write it as an arrow function like here
  handleAddressChange = (event: StripeAddressElementChangeEvent) => {
    // this.completionStatus.update((state) => {
    //   state.address = event.complete;
    //   return state;
    // });
    this.addressComplete.set(event.complete);
  };

  // Here we don't need to bind the method to the class using an arrow function because we are not using it inside the ngOnInit life cycle hook, but rather use it in the template - In the app-checkout-delivery element that contains the deliveryComplete field which is defined as output, we can write '(deliveryComplete)="handleDeliverChange($event)"' which means that we are calling the handleDeliverChange method and passing it the deliveryComplete value
  handleDeliverChange(event: boolean) {
    // this.completionStatus.update((state) => {
    //   state.delivery = event;
    //   return state;
    // });
    this.deliveryComplete.set(event);
  }

  async getConfirmationToken() {
    try {
      // We want first to check our completion parameters are fulfilled before running this function
      // If all 3 parameters werer inside an object we could run the following check (this is the same):
      // if (Object.values(this.completionStatus()).every(status => status === true)) - This will check all the values in the object and return if they are all true
      if (this.addressComplete() && this.deliveryComplete() && this.paymentComplete()) {
        const result = await this.stripeService.createConfirmationToken();
        if (result.error) {
          throw new Error(result.error.message);
        }
        this.confirmationToken.set(result.confirmationToken);
        console.log(this.confirmationToken());
      }
    } catch (error: any) {
      this.snackbar.error(error.message);
    }
  }

  // Hovering on the (selectionChange) in the template file will show what is the selectionChange and what it returns. The (selectionChange) type is an EventEmitter of type StepperSelectionEvent which means that it emits an event whenever a step is changed. The idea here is that in the first step, if the user changes whether to save the filled address or not he will change the saveAddress property. When clicking on the Next button the application will check if this property is changed and if so and the checkbox is checked, the application will update the address from the stripe fields to our DB. Since the EventEmitter emits a StepperSelectionEvent, this is the type of the event specified here
  async onStepChange(event: StepperSelectionEvent) {
    // The selectedIndex indicates the step we are in (starting from 0). Step 1 means that we moved from the step where the user fills his address
    if (event.selectedIndex === 1) {
      if (this.saveAddress) {
        const address = await this.getAddressFromStripeAddress();
        address && firstValueFrom(this.accounbtService.updateAddress(address));
      }
    }
    if (event.selectedIndex === 2) {
      await firstValueFrom(this.stripeService.createOrUpdatePaymentIntent());
    }
    if (event.selectedIndex === 3) {
      await this.getConfirmationToken();
    }
  }

  // We inject the stepper to this function because we want to take the user back to the payment step if there is a problem with the payment. Since we can only use the confirmation token once we will take the user to the payment step where a new token will be generated for him
  async confirmPayment(stepper: MatStepper) {
    this.loading.set(true);
    try {
      if (this.confirmationToken()) {
        const result = await this.stripeService.confirmPayment(this.confirmationToken()!);
        if (result.error) {
          throw new Error(result.error.message);
        } else {
          // If the payment was successful -> reseting the cart and the delivery method and navigating to a success page
          this.cartService.deleteCart();
          this.cartService.selectedDelivery.set(null);
          this.router.navigateByUrl('/checkout/success');
        }
      }
    } catch (error: any) {
      this.snackbar.error(error.message || 'Something went wrong');
      // Taking the user one step back to start the payment process again
      stepper.previous();
    } finally {
      this.loading.set(false);
    }
  }

  // Here we define that we return a Promise of type Address from our shared/models
  private async getAddressFromStripeAddress(): Promise<Address | null> {
    const result = await this.addressElement?.getValue();
    const address = result?.value.address;
    if (address) {
      return {
        line1: address.line1,
        line2: address.line2 || undefined,
        city: address.city,
        state: address.state,
        country: address.country,
        postalCode: address.postal_code,
      };
    } else return null;
  }

  onSaveAddressCheckboxChange(event: MatCheckboxChange) {
    this.saveAddress = event.checked;
  }

  ngOnDestroy(): void {
    this.stripeService.disposeElements();
  }
}
