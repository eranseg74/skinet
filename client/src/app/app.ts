import { Component, inject, OnInit, signal } from '@angular/core';
// import { RouterOutlet } from '@angular/router';
import { Header } from './layout/header/header';
import { HttpClient } from '@angular/common/http';
import { Product } from './shared/models/product';
import { Pagination } from './shared/models/pagination';

@Component({
  selector: 'app-root',
  imports: [/*RouterOutlet, */ Header],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App implements OnInit {
  baseUrl = 'https://localhost:5001/api/';
  private http = inject(HttpClient); // This is like injecting the HttpClient in the constructor
  protected readonly title = signal('Skinet');
  products: Product[] = [];

  ngOnInit(): void {
    // In this method it is recommended to execute the http requests. In the constructor it might be too soon.
    // The http request returns an Observable so we are subscribing to it in order to do something with the response.
    // Ususally we neet to unsubscribe when observing is no londer needed. In http request we don't have to specifically unsubscribe because for http requests, after getting a response the subscription is terminated automatically.
    this.http.get<Pagination<Product>>(this.baseUrl + 'products').subscribe({
      next: (response) => {
        this.products = response.data;
        console.log(response);
      },
      error: (error) => console.log(error),
      complete: () => console.log('complete'),
    });
  }
}
