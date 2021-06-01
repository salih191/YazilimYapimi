import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Currency } from 'src/app/models/currencies/currency';
import { AuthService } from 'src/app/services/auth.service';
import { CurrencyService } from 'src/app/services/currency.service';
import { WalletService } from 'src/app/services/wallet.service';

@Component({
  selector: 'app-money-add',
  templateUrl: './money-add.component.html',
  styleUrls: ['./money-add.component.css']
})
export class MoneyAddComponent implements OnInit {

  moneyAddForm: FormGroup;
  currencies:Currency[]
  constructor(
    private formBuilder: FormBuilder,
    private walletService: WalletService,
    private toastrService: ToastrService,
    private authService:AuthService,
    private currencyService:CurrencyService
  ) {}

  ngOnInit(): void {
    this.createMoneyAddForm();
    this.currencyService.getCurrency().subscribe(data=>{
      this.currencies=data.data
    })
  }

  createMoneyAddForm() {
    this.moneyAddForm = this.formBuilder.group({
      amount: ['', Validators.required],
      userId:Number(this.authService.getUser().id),
      currencyId:['',Validators.required]
    });
  }
  add() {
    console.log(this.moneyAddForm.value)
    if (this.moneyAddForm.valid) {
      let moneyModel = Object.assign({}, this.moneyAddForm.value);
      moneyModel.currencyId=Number(moneyModel.currencyId)
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
