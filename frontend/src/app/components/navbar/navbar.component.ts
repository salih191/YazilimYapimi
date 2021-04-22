import { Component, OnInit } from '@angular/core';
import { ProductDto } from 'src/app/models/products/productDto';
import { User } from 'src/app/models/user/user';
import { WalletDto } from 'src/app/models/wallet/walletDto';
import { AdminService } from 'src/app/services/admin.service';
import { AuthService } from 'src/app/services/auth.service';
import { WalletService } from 'src/app/services/wallet.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
})
export class NavbarComponent implements OnInit {
  girisYapili: boolean;
  user: User;
  wallet: number;
  admin: boolean;
  onaybekleyenUrun: ProductDto[];
  onaybekleyenPara: WalletDto[];
  onaylanacakVarmi:boolean=false
  constructor(
    private authService: AuthService,
    private walletService: WalletService,
    private adminService: AdminService
  ) {}

  ngOnInit(): void {
    this.girisYapili = this.authService.isAuthenticated();
    if (this.girisYapili) {
      this.admin = this.authService.isUserHaveClaims(['admin']);//admin rölüne sahip mi
      this.user = this.authService.getUser();
      this.walletService.getWallet(this.user.id).subscribe(
        (response) => {
          this.wallet = response.data.amount;
        },
        (errorResponse) => {
          console.log(errorResponse);
        }
      );
      if (this.admin) {//adminse onaylanacakları getir
        this.adminService.getAaddMoney().subscribe(
          (response) => {
            this.onaybekleyenPara = response.data;
            if(response.data.length>0)
            this.onaylanacakVarmi=true
          },
          (errorResponse) => {
            console.log(errorResponse);
          }
        );
        this.adminService.getAaddProduct().subscribe(
          (response) => {
            console.log(response);
            this.onaybekleyenUrun = response.data;
            if(response.data.length>0)
            this.onaylanacakVarmi=true
          },
          (errorResponse) => {
            console.log(errorResponse);
          }
        );
      }
    }
  }

  logOut() {//çıkış yap
    this.authService.logout();
  }
  confirmProduct(product: ProductDto) {
    console.log(product);
    this.adminService.confirmAaddProduct(product).subscribe(
      (response) => {
        window.location.reload();
      },
      (erorrResponse) => {
        console.log(erorrResponse);
      }
    );
  }
  rejectProduct(product: ProductDto) {
    console.log(product);
    this.adminService.rejectAaddProduct(product).subscribe(
      (response) => {
        window.location.reload();
      },
      (erorrResponse) => {
        console.log(erorrResponse);
      }
    );
  }

  confirmMoney(wallet: WalletDto) {
    this.adminService.confirmAaddMoney(wallet).subscribe(
      (response) => {
        window.location.reload();
      },
      (erorrResponse) => {
        console.log(erorrResponse);
      }
    );
  }
  rejectMoney(wallet: WalletDto) {
    this.adminService.rejectAaddMoney(wallet).subscribe(
       (response) => {
        window.location.reload();
      },
      (erorrResponse) => {
        console.log(erorrResponse);
      }
    );
  }
}
