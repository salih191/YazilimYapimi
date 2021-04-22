import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { ToastrModule } from 'ngx-toastr';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { HomePageComponent } from './components/home-page/home-page.component';
import { MoneyAddComponent } from './components/money-add/money-add.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { OrderAddComponent } from './components/order-add/order-add.component';
import { ProductAddComponent } from './components/product-add/product-add.component';
import { ReplacePipe } from './pipes/replace.pipe';
import { CategoryAddComponent } from './components/category-add/category-add.component';
import { AuthInterceptor } from './interceptors/auth';


@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    RegisterComponent,
    HomePageComponent,
    MoneyAddComponent,
    NavbarComponent,
    OrderAddComponent,
    ProductAddComponent,
    ReplacePipe,
    CategoryAddComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    ToastrModule.forRoot({
      positionClass: 'toast-bottom-right',
    }),
  ],
  providers: [{
    provide: HTTP_INTERCEPTORS,useClass:AuthInterceptor,multi:true
  }],
  bootstrap: [AppComponent]
})
export class AppModule { }
