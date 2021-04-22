import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiUrlHelper } from '../helpers/apiHelpers';
import { ListResponseModel } from '../models/listResponseModel';
import { Product } from '../models/products/product';
import { ProductDto } from '../models/products/productDto';
import { ResponseModel } from '../models/responseModel';
import { Wallet } from '../models/wallet/wallet';
import { WalletDto } from '../models/wallet/walletDto';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  constructor(private httpClient: HttpClient) { }

    getAaddMoney():Observable<ListResponseModel<WalletDto>>{
      return this.httpClient.get<ListResponseModel<WalletDto>>(ApiUrlHelper.getUrl("admin/getaddmoney"))
    }
    confirmAaddMoney(addMoney:WalletDto):Observable<ResponseModel>{
      return this.httpClient.post<ListResponseModel<WalletDto>>(ApiUrlHelper.getUrl("admin/confirmaddmoney"),addMoney)
    }
    rejectAaddMoney(addMoney:WalletDto):Observable<ResponseModel>{
      return this.httpClient.post<ListResponseModel<WalletDto>>(ApiUrlHelper.getUrl("admin/rejectaddmoney"),addMoney)
    }

    getAaddProduct():Observable<ListResponseModel<ProductDto>>{
      return this.httpClient.get<ListResponseModel<ProductDto>>(ApiUrlHelper.getUrl("admin/getaddproduct"))
    }
    confirmAaddProduct(addProduct:ProductDto):Observable<ResponseModel>{
      return this.httpClient.post<ListResponseModel<Product>>(ApiUrlHelper.getUrl("admin/confirmaddproduct"),addProduct)
    }
    rejectAaddProduct(addProduct:ProductDto):Observable<ResponseModel>{
      return this.httpClient.post<ListResponseModel<Product>>(ApiUrlHelper.getUrl("admin/rejectaddproduct"),addProduct)
    }
}
