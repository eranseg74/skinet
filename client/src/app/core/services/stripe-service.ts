import { inject, Injectable } from '@angular/core';
import {
  ConfirmationToken,
  loadStripe,
  Stripe,
  StripeAddressElement,
  StripeAddressElementOptions,
  StripeElements,
  StripePaymentElement,
} from '@stripe/stripe-js';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { CartService } from './cart-service';
import { Cart } from '../../shared/models/cart';
import { firstValueFrom, map } from 'rxjs';
import { AccountService } from './account-service';

@Injectable({
  providedIn: 'root',
})
export class StripeService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  private cartService = inject(CartService);
  private accountService = inject(AccountService);
  // The ? sign will assign undefined if there is no promise and we need a null value so we write it like this:
  private stripePromise: Promise<Stripe | null>;
  // The following properties are elements for stripe. We are creating all the stripe elements as optional because we will initialize it later. We also creating them inside the service because we want to reuse the same instance of elements throughout the application.
  private elements?: StripeElements;
  private addressElement?: StripeAddressElement;
  private paymentElement?: StripePaymentElement;

  constructor() {
    this.stripePromise = loadStripe(environment.stripePublicKey);
  }

  getStripeInstance() {
    return this.stripePromise;
  }

  async initializeElements() {
    if (!this.elements) {
      const stripe = await this.getStripeInstance(); // Return Stripe or null
      if (stripe) {
        // The createOrUpdatePaymentIntent returns an Observable<Cart> so to get the cart itself we can use the firstValueFrom function which converts an observable to a promise by subscribing to the observable, and returning a promise that will resolve as soon as the first value arrives from the observable. The subscription will then be closed.
        const cart = await firstValueFrom(this.createOrUpdatePaymentIntent());
        this.elements = stripe.elements({
          clientSecret: cart.clientSecret,
          appearance: { labels: 'floating' },
        });
      } else {
        throw new Error('Stripe has not been loaded');
      }
    }
    return this.elements;
  }

  async createPaymentElement() {
    if (!this.paymentElement) {
      const elements = await this.initializeElements();
      if (elements) {
        this.paymentElement = elements.create('payment');
      } else {
        throw new Error('Elements instance has not been initialized');
      }
    }
    return this.paymentElement;
  }

  // Defining the method as async because stripe usually return promises
  async createAddressElement() {
    if (!this.addressElement) {
      const elements = await this.initializeElements();
      if (elements) {
        const user = this.accountService.currentUser();
        let defaultValues: StripeAddressElementOptions['defaultValues'] = {};
        if (user) {
          defaultValues.name = user.firstName + ' ' + user.lastName;
        }
        if (user?.address) {
          defaultValues.address = {
            line1: user.address.line1,
            line2: user.address.line2,
            city: user.address.city,
            state: user.address.state,
            country: user.address.country,
            postal_code: user.address.postalCode,
          };
        }
        const options: StripeAddressElementOptions = {
          mode: 'shipping', // Two options here are billing and shipping. If we choose shipping Stripe will prompt a question if we want to use the shipping address for billing. If not, Stripe will ask the user to define a billing address
          defaultValues,
        };
        this.addressElement = elements.create('address', options);
      } else {
        throw new Error('Elements instance has not been loaded');
      }
    }
    return this.addressElement;
  }

  async createConfirmationToken() {
    const stripe = await this.getStripeInstance();
    const elements = await this.initializeElements();
    const result = await elements.submit(); // Before confirming payment, call elements.submit() to validate the state of the Payment Element and collect any data required for wallets.
    if (result.error) throw new Error(result.error.message);
    if (stripe) {
      // Checking that we have stripe and it is not null
      return await stripe.createConfirmationToken({ elements }); // Creating a confirmation token for the payment. This function gets an Option object. In this case we pass the elements inside an object notation
    } else {
      throw new Error('Stripe not available');
    }
  }

  async confirmPayment(confirmationToken: ConfirmationToken) {
    const stripe = await this.getStripeInstance();
    const elements = await this.initializeElements();
    const result = await elements.submit(); // Before confirming payment, call elements.submit() to validate the state of the Payment Element and collect any data required for wallets.
    if (result.error) throw new Error(result.error.message);

    const clientSecret = this.cartService.cart()?.clientSecret;
    if (stripe && clientSecret) {
      // The confirmPayment method (from Stripe) expectss a 'return_url' property in order to redirect the user once the confirmation is complete. Since we do not include that parameter we define outside the confirmParams object: redirect: 'if_required' which means that stripe.confirmPayment will only redirect if we chooses a redirect-based payment method. Setting if_required requires that we handle successful confirmations for redirect-based and non-redirect based payment methods separately. If we don't specify this line we will get an error
      return await stripe.confirmPayment({
        clientSecret: clientSecret,
        confirmParams: {
          confirmation_token: confirmationToken.id,
        },
        redirect: 'if_required',
      });
    } else {
      throw new Error('Unable to load stripe');
    }
  }

  createOrUpdatePaymentIntent() {
    const cart = this.cartService.cart();
    if (!cart) throw new Error('Problem with cart');
    // Form the next call we will get the cart with the paymentId and the ClientSecret
    return this.http.post<Cart>(this.baseUrl + 'payments/' + cart.id, {}).pipe(
      map((cart) => {
        // In the cart service we have the setCart which updates the cart in the Redis DB, and the set function that sets the cart signal. Here we use the setCart function because it also updates the Redis DB
        this.cartService.setCart(cart);
        return cart;
      })
    );
  }

  // Since we implement stripe as a service, at start, when the first user logs in the elements are null so stripe will fetch the correct address of the logged in user. Once the user logs out and another user logs in, the elements are filled so the address will not be updated. The new logged in user will see the address of the first user. To handle this we create here a method for reseting the elements and addressElement. We will call this method in the ngOnDestroy life cycle hook method in the checkout component so whenever a user leaves the checkout (the checkout component is destroyed) these stripe elements will reset. Also good to see the correct address after updating it
  disposeElements() {
    this.elements = undefined;
    this.addressElement = undefined;
    this.paymentElement = undefined;
  }
}
