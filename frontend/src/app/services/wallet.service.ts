import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiUrlHelper } from '../helpers/apiHelpers';
import { SingleResponseModel } from '../models/singleResponseModel';
import { Wallet } from '../models/wallet/wallet';

@Injectable({
  providedIn: 'root'
})
export class WalletService {

  getWalletPath: string = 'wallet/getbyuserId';
  constructor(private httpClient: HttpClient) { }

    getWallet(userId:number):Observable<SingleResponseModel<Wallet>>{
      return this.httpClient.get<SingleResponseModel<Wallet>>(ApiUrlHelper.getUrlWithParameters(this.getWalletPath,[{value:userId,key:"userId"}]))
    }

    addWallet(wallet:Wallet):Observable<SingleResponseModel<Wallet>>{
      return this.httpClient.post<SingleResponseModel<Wallet>>(ApiUrlHelper.getUrl("wallet/add"),wallet)
    }
}
