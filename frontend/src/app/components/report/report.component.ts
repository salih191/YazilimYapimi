import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ApiUrlHelper } from 'src/app/helpers/apiHelpers';
import { AuthService } from 'src/app/services/auth.service';
import { ReportService } from 'src/app/services/report.service';

@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  styleUrls: ['./report.component.css']
})
export class ReportComponent implements OnInit {

  reportForm:FormGroup
  constructor(private formBuilder: FormBuilder,private reportService:ReportService,private authService:AuthService) { }
  startTime:Date

  userId=this.authService.getUser().id
  ngOnInit(): void {
    this.createReportForm()
  }
  createReportForm() {
    this.reportForm = this.formBuilder.group({
      userId:this.userId,
      startTime: ['', Validators.required],
      endTime: ['', Validators.required],
    });
  }
  create(){
    if(this.reportForm.valid){
      let reportModel = Object.assign({}, this.reportForm.value);
      reportModel.userId=Number(this.userId)
      console.log(reportModel)
     this.reportService.report(reportModel).subscribe(data=>{
      window.location.href=ApiUrlHelper.getUrlWithParameters("report/report.csv",[{key:"id",value:this.userId}])
     },d2=>{
       console.log("d2",d2)
     })
    }
  }
}
