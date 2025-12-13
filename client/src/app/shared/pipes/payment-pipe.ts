import { Pipe, PipeTransform } from '@angular/core';
import { ConfirmationToken } from '@stripe/stripe-js';

@Pipe({
  name: 'payment',
})
export class PaymentPipe implements PipeTransform {
  transform(
    value: ConfirmationToken['payment_method_preview']['card'],
    ...args: unknown[]
  ): unknown {
    if (value?.brand && value.last4 && value.exp_month && value.exp_year) {
      const { brand, last4, exp_month, exp_year } = value;
      return `${brand.toUpperCase()} **** **** **** ${last4}, Exp: ${exp_month}/${exp_year}`;
    } else {
      return 'Missing card details';
    }
  }
}
