import { Component, inject, NgZone } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatBadge } from '@angular/material/badge';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { BusyService } from '../../core/services/busy-service';
import { MatProgressBar } from '@angular/material/progress-bar';
import { CartService } from '../../core/services/cart-service';
import { AccountService } from '../../core/services/account-service';
import { MatMenu, MatMenuItem, MatMenuTrigger } from '@angular/material/menu';
import { MatDivider } from '@angular/material/divider';

@Component({
  selector: 'app-header',
  imports: [
    MatIconModule,
    MatButtonModule,
    MatBadge,
    RouterLink,
    RouterLinkActive,
    MatProgressBar,
    MatMenuTrigger,
    MatMenu,
    MatDivider,
    MatMenuItem,
  ],
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class Header {
  protected busyService = inject(BusyService);
  protected cartService = inject(CartService);
  protected accountService = inject(AccountService);
  private router = inject(Router);

  logout() {
    this.accountService.logout().subscribe({
      next: () => {
        this.accountService.currentUser.set(null);
        this.router.navigateByUrl('/');
      },
    });
  }
}
