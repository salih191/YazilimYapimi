import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable, timer } from 'rxjs';
import { LoginModel } from '../models/login-register/loginModel';
import { RegisterModel } from '../models/login-register/registerModel';
import { SingleResponseModel } from '../models/singleResponseModel';
import { TokenService } from './token.service';
import jwt_decode from "jwt-decode";
import { TokenModel } from '../models/token/tokenModel';
import { ApiUrlHelper } from '../helpers/apiHelpers';
import { User } from '../models/user/user';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(
    private httpClient: HttpClient,
    private toastrService: ToastrService,
    private tokenService: TokenService,
    private router: Router
  ) {}

  getLoginPath: string = 'auth/Login';
  getRegisterPath: string = 'auth/Register';

  login(loginModel: LoginModel): Observable<SingleResponseModel<TokenModel>> {
    return this.httpClient.post<SingleResponseModel<TokenModel>>(
      ApiUrlHelper.getUrl(this.getLoginPath),
      loginModel
    );
  }

  register(
    registerModel: RegisterModel
  ): Observable<SingleResponseModel<TokenModel>> {
    return this.httpClient.post<SingleResponseModel<TokenModel>>(
      ApiUrlHelper.getUrl(this.getRegisterPath),
      registerModel
    );
  }

  isUserHaveClaims(necessaryClaims: string[]): boolean {
    if (necessaryClaims == undefined) return false;
    if(this.isAuthenticated()==false){
      return false;
    }
    let isUserHaveClaim: boolean = false;
    
    let claims:string=JSON.parse(JSON.stringify(jwt_decode(this.tokenService.getToken()))).roles
    claims.split(",").forEach((ownedClaim) => {
      console.log(claims)
      necessaryClaims.forEach((necessaryClaim) => {
        if (ownedClaim === 'admin' || ownedClaim === necessaryClaim) {
          isUserHaveClaim = true;
        }
      });
    });
    return isUserHaveClaim;
  }

  isAuthenticated(): boolean {
    return this.tokenService.tokenExist();
  }

  getUser(): User {
    let token:string=this.tokenService.getToken()
    let user: User = jwt_decode(token)
    return user;
  }

  logout() {
    this.tokenService.delete();
    this.toastrService.success('Başarıyla çıkış yaptınız.');
    timer(1000).subscribe(p=>{
      window.location.reload();
    });
  }
}