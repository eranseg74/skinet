import { Component, inject, OnInit, signal } from '@angular/core';
import { OrderService } from '../../../core/services/order-service';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { Order } from '../../../shared/models/order';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { AddressPipe } from '../../../shared/pipes/address-pipe';
import { PaymentPipe } from '../../../shared/pipes/payment-pipe';
import { MatIcon } from '@angular/material/icon';

@Component({
  selector: 'app-order-detailed',
  imports: [
    MatCardModule,
    MatButtonModule,
    DatePipe,
    CurrencyPipe,
    AddressPipe,
    PaymentPipe,
    RouterLink,
  ],
  templateUrl: './order-detailed.html',
  styleUrl: './order-detailed.scss',
})
export class OrderDetailed implements OnInit {
  private orderService = inject(OrderService);
  // Allows access to the route parameters from the URL:
  private activatedRoute = inject(ActivatedRoute);
  order = signal<Order | null>(null);

  ngOnInit(): void {
    this.loadOrder();
  }

  loadOrder() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (!id) return;
    this.orderService.getOrderDetailed(+id).subscribe({
      next: (order) => this.order.set(order),
    });
  }
}
