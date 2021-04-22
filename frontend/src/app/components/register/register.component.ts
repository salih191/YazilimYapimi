import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { RegisterModel } from 'src/app/models/login-register/registerModel';
import { AuthService } from 'src/app/services/auth.service';
import { TokenService } from 'src/app/services/token.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  constructor(private formBuilder:FormBuilder,private authService:AuthService,private tokenService:TokenService,private toastrService:ToastrService,
    private router:Router) { }

  ngOnInit(): void {
    if(this.authService.isAuthenticated())//giriş yapılı mı
    {
      this.router.navigate([""])
    }
    this.createLoginForm();
  }

  registerForm:FormGroup;
  createLoginForm(){
    this.registerForm = this.formBuilder.group({
      firstName:new FormControl('',Validators.required),
      lastName:new FormControl('',Validators.required),
      userName:new FormControl('',Validators.required),
      tcIdentityNumber:new FormControl('',Validators.required),
      phoneNumber:new FormControl('',Validators.required),
      email: new FormControl('', Validators.required),
      password: new FormControl('', Validators.required),
      address: new FormControl('',Validators.required)
    });
  }
  register(){
    if(this.registerForm.valid){
      let registerDto : RegisterModel = Object.assign({},this.registerForm.value);
      this.authService.register(registerDto).subscribe(response=>{
        this.tokenService.setToken(response.data);
        this.toastrService.success(response.message);
        window.history.replaceState({},'',"")
        window.history.go(0)
        
      }, errorResponse=>{
        console.log(errorResponse)
        this.toastrService.error(errorResponse.error, 'Hata');
      });
    }
    else{
      this.toastrService.warning('Lütfen gerekli alanları istenilen biçimde doldurunuz.','Model Doğrulama Uyarısı');
    }
  }
}
