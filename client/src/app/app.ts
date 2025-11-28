import { Component, inject, OnInit, signal } from '@angular/core';
// import { RouterOutlet } from '@angular/router';
import { Header } from './layout/header/header';
import { Product } from './shared/models/product';
import { Pagination } from './shared/models/pagination';
import { ShopService } from './core/shop-service';
import { Shop } from './features/shop/shop';

@Component({
  selector: 'app-root',
  imports: [/*RouterOutlet, */ Header, Shop],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected readonly title = signal('Skinet');
}
