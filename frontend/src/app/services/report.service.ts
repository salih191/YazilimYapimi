import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiUrlHelper } from '../helpers/apiHelpers';
import { ListResponseModel } from '../models/listResponseModel';
import { Report } from '../models/report/report';
import { ResponseModel } from '../models/responseModel';
import { SingleResponseModel } from '../models/singleResponseModel';

@Injectable({
  providedIn: 'root'
})
export class ReportService {

  constructor(private httpClient: HttpClient) { }
  report(report:Report): Observable<SingleResponseModel<Report>> {
    return this.httpClient.post<SingleResponseModel<Report>>(ApiUrlHelper.getUrl("report/report"),report
    );
  }
}
