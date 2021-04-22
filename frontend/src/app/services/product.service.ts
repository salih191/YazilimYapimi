import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiUrlHelper } from '../helpers/apiHelpers';
import { ListResponseModel } from '../models/listResponseModel';
import { Product } from '../models/products/product';
import { ProductDto } from '../models/products/productDto';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  constructor(private httpClient: HttpClient) { }

  getProductDetails(): Observable<ListResponseModel<ProductDto>> {
    return this.httpClient.get<ListResponseModel<ProductDto>>(ApiUrlHelper.getUrl("products/getallproductdetails")
    );
  }

  addProducts(product:Product): Observable<ListResponseModel<Product>> {
    return this.httpClient.post<ListResponseModel<Product>>(ApiUrlHelper.getUrl("products/add"),product);
  }
}
