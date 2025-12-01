import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormField, MatLabel } from '@angular/material/select';
import { MatInput } from '@angular/material/input';
import { CartService } from '../../../core/services/cart-service';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-order-summery',
  imports: [RouterLink, MatButtonModule, MatFormField, MatLabel, MatInput, CurrencyPipe],
  templateUrl: './order-summery.html',
  styleUrl: './order-summery.scss',
})
export class OrderSummery {
  protected cartSrvice = inject(CartService);
}
