import { Component, inject, OnInit } from '@angular/core';
import { ShopService } from '../../../core/services/shop-service';
import { ActivatedRoute } from '@angular/router';
import { Product } from '../../../shared/models/product';
import { CurrencyPipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormField, MatLabel } from '@angular/material/select';
import { MatInput } from '@angular/material/input';
import { MatDividerModule } from '@angular/material/divider';

@Component({
  selector: 'app-product-details',
  imports: [
    CurrencyPipe,
    MatButtonModule,
    MatIconModule,
    MatFormField,
    MatInput,
    MatLabel,
    MatDividerModule,
  ],
  templateUrl: './product-details.html',
  styleUrl: './product-details.scss',
})
export class ProductDetails implements OnInit {
  private shopService = inject(ShopService);
  private activatedRoute = inject(ActivatedRoute);
  product?: Product;

  ngOnInit(): void {
    this.loadProduct();
  }

  loadProduct() {
    // We can get the snapshot of the active route and query it to get what we want from the request properties. In this case we get the route parameters which includes the id (as defined in the API call - shop/:id). The name must match the one specified in the appropriate path in the app.routes.ts file. We can get a null so we must check it. Also, everything that comes from the http call, comes as a string so we need to cast it to a number. We do so by adding the '+' sign before the variable.
    const id = this.activatedRoute.snapshot.paramMap.get('id');
    if (!id) {
      return;
    }
    this.shopService.getProduct(+id).subscribe({
      next: (product) => {
        console.log(product);
        this.product = product;
      },
      error: (error) => console.log(error),
    });
  }
}
