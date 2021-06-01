import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiUrlHelper } from '../helpers/apiHelpers';
import { Currency } from '../models/currencies/currency';
import { ListResponseModel } from '../models/listResponseModel';

@Injectable({
  providedIn: 'root'
})
export class CurrencyService {

  constructor(private httpClient: HttpClient) { }
  getCurrency(): Observable<ListResponseModel<Currency>> {
    return this.httpClient.get<ListResponseModel<Currency>>(ApiUrlHelper.getUrl("currencies/getall")
    );
  }
}
