import { Pipe, PipeTransform } from '@angular/core';
import { ConfirmationToken } from '@stripe/stripe-js';
import { ShippingAddress } from '../models/order';

@Pipe({
  name: 'address',
})
export class AddressPipe implements PipeTransform {
  transform(value?: ConfirmationToken['shipping'] | ShippingAddress, ...args: unknown[]): unknown {
    if (value && 'address' in value && value.name) {
      // Destructuring the address:
      // Putting the ! sign to overcome the Property 'postal_code' does not exist on type 'Address | undefined' error
      const { line1, line2, city, state, country, postal_code } = (
        value as ConfirmationToken['shipping']
      )?.address!;
      return `${value.name}, ${line1}${line2 ? ', ' + line2 : ''}, ${city}${
        state ? ', ' + state : ''
      }, ${postal_code}, ${country}`;
      // Although line1 does exist in the confirmation token it is in the value.address and not directly under the value itself
    } else if (value && 'line1' in value) {
      const { line1, line2, city, state, country, postalCode } = value as ShippingAddress;
      return `${value.name}, ${line1}${line2 ? ', ' + line2 : ''}, ${city}${
        state ? ', ' + state : ''
      }, ${postalCode}, ${country}`;
    } else {
      return 'Unknown address';
    }
  }
}
