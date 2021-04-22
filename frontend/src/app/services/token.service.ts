import { Injectable } from '@angular/core';
import { TokenModel } from '../models/token/tokenModel';
import { CookieService } from 'ngx-cookie-service';


@Injectable({
  providedIn: 'root'
})
export class TokenService {

  constructor(private cookieService:CookieService) { }

  cacheKey: string = 'token';

  setToken(accessToken: TokenModel) {//tokeni çerezlere ekle
    this.cookieService.set(this.cacheKey,accessToken.token)
  }

  getToken(): string {//çerezlerden getir
    let token:string=this.cookieService.get(this.cacheKey)
    return token;
  }

  tokenExist(): boolean {//token var mı
    if (this.cookieService.get(this.cacheKey)) return true;
    else return false;
  }

  delete() {//çerezlerden tokeni sil
    this.cookieService.delete(this.cacheKey)
  }
}
