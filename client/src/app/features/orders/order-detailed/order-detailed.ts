import { Component, inject, OnInit, signal } from '@angular/core';
import { OrderService } from '../../../core/services/order-service';
import { ActivatedRoute, Router } from '@angular/router';
import { Order } from '../../../shared/models/order';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { AddressPipe } from '../../../shared/pipes/address-pipe';
import { PaymentPipe } from '../../../shared/pipes/payment-pipe';
import { AccountService } from '../../../core/services/account-service';
import { AdminService } from '../../../core/services/admin-service';

@Component({
  selector: 'app-order-detailed',
  imports: [MatCardModule, MatButtonModule, DatePipe, CurrencyPipe, AddressPipe, PaymentPipe],
  templateUrl: './order-detailed.html',
  styleUrl: './order-detailed.scss',
})
export class OrderDetailed implements OnInit {
  private orderService = inject(OrderService);
  // Allows access to the route parameters from the URL:
  private activatedRoute = inject(ActivatedRoute);
  private accountService = inject(AccountService);
  private adminService = inject(AdminService);
  private router = inject(Router);
  order = signal<Order | null>(null);
  protected buttonText = this.accountService.isAdmin() ? 'Return to admin' : 'Return to orders';

  ngOnInit(): void {
    this.loadOrder();
  }

  onReturnClick() {
    const path = this.accountService.isAdmin() ? '/admin' : '/orders';
    this.router.navigateByUrl(path);
  }

  loadOrder() {
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (!id) return;
    const loadOrderData = this.accountService.isAdmin() // The loadOrderData will get an Observable
      ? this.adminService.getOrder(+id)
      : this.orderService.getOrderDetailed(+id);
    loadOrderData.subscribe({
      // Subscribing to an Observable whether it comes from the admin service or from the order service according to the user's role
      next: (order) => this.order.set(order),
    });
  }
}
