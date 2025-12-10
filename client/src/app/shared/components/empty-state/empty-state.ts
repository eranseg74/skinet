import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { CartService } from '../../../core/services/cart-service';

@Component({
  selector: 'app-empty-state',
  imports: [MatIcon, MatButtonModule, RouterLink],
  templateUrl: './empty-state.html',
  styleUrl: './empty-state.scss',
})
export class EmptyState {
  protected cartService = inject(CartService);
}
