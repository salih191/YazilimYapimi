import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiUrlHelper } from '../helpers/apiHelpers';
import { Category } from '../models/category/category';
import { ListResponseModel } from '../models/listResponseModel';
import { ResponseModel } from '../models/responseModel';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  constructor(private httpClient: HttpClient) { }

  getCategory(): Observable<ListResponseModel<Category>> {
    return this.httpClient.get<ListResponseModel<Category>>(ApiUrlHelper.getUrl("categories/getall")
    );
  }

  addCategory(category:Category): Observable<ResponseModel> {
    return this.httpClient.post<ListResponseModel<Category>>(ApiUrlHelper.getUrl("categories/add"),category);
  }
}
