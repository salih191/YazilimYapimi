import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiUrlHelper } from '../helpers/apiHelpers';
import { ListResponseModel } from '../models/listResponseModel';
import { Order } from '../models/order/order';
import { OrderDetailsDto } from '../models/order/orderDetailsDto';
import { ResponseModel } from '../models/responseModel';

@Injectable({
  providedIn: 'root'
})
export class OrderService {

  constructor(private httpClient: HttpClient) { }

  getOrders(): Observable<ListResponseModel<Order>> {
    return this.httpClient.get<ListResponseModel<Order>>(ApiUrlHelper.getUrl("orders/getall")
    );
  }

  getOrderDetails(): Observable<ListResponseModel<OrderDetailsDto>> {
    return this.httpClient.get<ListResponseModel<OrderDetailsDto>>(ApiUrlHelper.getUrl("orders/getalldetail")
    );
  }

  addOrders(order:Order): Observable<ResponseModel> {
    return this.httpClient.post<ResponseModel>(ApiUrlHelper.getUrl("orders/add"),order
    );
  }
}
