import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-report',
  templateUrl: './report.component.html',
  styleUrls: ['./report.component.css']
})
export class ReportComponent implements OnInit {

  reportForm:FormGroup
  constructor(private formBuilder: FormBuilder,) { }
  startDate:Date
  endDate:Date
  nowDate:Date
  ngOnInit(): void {
    console.log(this.nowDate)
    this.createReportForm()
  }
  createReportForm() {
    this.reportForm = this.formBuilder.group({
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
    });
  }
  create(){
    console.log(this.startDate)
    if(this.reportForm.valid){
     
      console.log(this.startDate.getDate(),this.nowDate.toString(),this.endDate.getDate)
     // window.location.href='https://www.gencayyildiz.com/blog/jquery-ile-yonlendirmeredirect-islemi/'
    }
  }
}
