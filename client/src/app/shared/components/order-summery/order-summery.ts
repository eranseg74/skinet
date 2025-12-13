import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormField, MatLabel } from '@angular/material/select';
import { MatInput } from '@angular/material/input';
import { CartService } from '../../../core/services/cart-service';
import { CurrencyPipe, Location } from '@angular/common';

@Component({
  selector: 'app-order-summery',
  imports: [RouterLink, MatButtonModule, MatFormField, MatLabel, MatInput, CurrencyPipe],
  templateUrl: './order-summery.html',
  styleUrl: './order-summery.scss',
})
export class OrderSummery {
  protected cartSrvice = inject(CartService);
  // The Location service can be used to interact with a browser's URL. To get data. To go to a different URL use the Router
  location = inject(Location); // Make sure the Location comes from @angular/common!!!
}
