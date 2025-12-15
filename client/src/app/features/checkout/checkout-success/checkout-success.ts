import { Component, inject, OnDestroy } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { SignalrService } from '../../../core/services/signalr-service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { AddressPipe } from '../../../shared/pipes/address-pipe';
import { PaymentPipe } from '../../../shared/pipes/payment-pipe';
import { OrderService } from '../../../core/services/order-service';

@Component({
  selector: 'app-checkout-success',
  imports: [
    MatButtonModule,
    RouterLink,
    MatProgressSpinnerModule,
    DatePipe,
    AddressPipe,
    CurrencyPipe,
    PaymentPipe,
  ],
  templateUrl: './checkout-success.html',
  styleUrl: './checkout-success.scss',
})
export class CheckoutSuccess implements OnDestroy {
  protected signalrService = inject(SignalrService);
  private orderService = inject(OrderService);

  ngOnDestroy(): void {
    this.orderService.orderComplete = false;
    // No need to keep the order in the signalR here because in this stage we already creating an API call to get the order so we are reseting the SignalR.
    // We want to create a guard that will not allow access to this component directly from the API. When loading this component the order signal is still null because it is waiting for the notification from the API on the successful payment from stripe so we cannot trust it to not be null in the guard. This is why we need the orderComplete flag
    this.signalrService.orderSignal.set(null);
  }
}
