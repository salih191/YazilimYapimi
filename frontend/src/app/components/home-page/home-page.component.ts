import { Component, OnInit } from '@angular/core';
import { ProductDto } from 'src/app/models/products/productDto';
import { ProductService } from 'src/app/services/product.service';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.css']
})
export class HomePageComponent implements OnInit {

  products: ProductDto[]
  dataLoaded=false
  constructor(private productService:ProductService) { }

  ngOnInit(): void {
    this.getProducts()
  }
  getProducts() {
    this.productService.getProductDetails().subscribe((response) => {//product servis üzerinden ürünler geliyor
      this.products = response.data;
      this.dataLoaded=true;
    })
  }
}
