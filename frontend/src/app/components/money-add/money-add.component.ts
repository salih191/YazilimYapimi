import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/services/auth.service';
import { WalletService } from 'src/app/services/wallet.service';

@Component({
  selector: 'app-money-add',
  templateUrl: './money-add.component.html',
  styleUrls: ['./money-add.component.css']
})
export class MoneyAddComponent implements OnInit {

  moneyAddForm: FormGroup;
  constructor(
    private formBuilder: FormBuilder,
    private walletService: WalletService,
    private toastrService: ToastrService,
    private authService:AuthService,
    private router:Router
  ) {}

  ngOnInit(): void {
    this.createMoneyAddForm();
  }

  createMoneyAddForm() {
    this.moneyAddForm = this.formBuilder.group({
      amount: ['', Validators.required],
      userId:Number(this.authService.getUser().id)
    });
  }
  add() {
    if (this.moneyAddForm.valid) {
      let moneyModel = Object.assign({}, this.moneyAddForm.value);
      this.walletService.addWallet(moneyModel).subscribe(
        (data) => {
          this.toastrService.success(data.message, 'Başarılı')
        },
        (dataError) => {
          if(dataError.error.ValidationErrors.length>0){
            for (let i = 0; i < dataError.error.ValidationErrors.length; i++) {
              this.toastrService.error(dataError.error.ValidationErrors[i].ErrorMessage, 'Hata');
            }
          }
        }
      );
    } else {
      this.toastrService.error('Formunuz eksik', 'Dikkat');
    }
  }

}
