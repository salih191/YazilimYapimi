import { Component, OnInit } from '@angular/core';
import {
  FormGroup,
  FormBuilder,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Category } from 'src/app/models/category/category';
import { Product } from 'src/app/models/products/product';
import { AuthService } from 'src/app/services/auth.service';
import { CategoryService } from 'src/app/services/category.service';
import { ProductService } from 'src/app/services/product.service';

declare var $:any;
@Component({
  selector: 'app-product-add',
  templateUrl: './product-add.component.html',
  styleUrls: ['./product-add.component.css'],
})
export class ProductAddComponent implements OnInit {
  productAddForm: FormGroup;
  categories: Category[];
  selectCategory:string
  constructor(
    private formBuilder: FormBuilder,
    private productService: ProductService,
    private toastrService: ToastrService,
    private authService: AuthService,
    private categoryService: CategoryService
  ) {}

  ngOnInit(): void {
    this.createProductAddForm();
    this.categoryService.getCategory().subscribe(
      (response) => {
        this.categories = response.data;
      },
      (errorResponse) => {
        console.log(errorResponse);
      }
    );
  }

  createProductAddForm() {
    this.productAddForm = this.formBuilder.group({
      unitPrice: ['', Validators.required],
      quantity: ['', Validators.required],
      categoryId: Number(this.selectCategory),
      supplierId: Number(this.authService.getUser().id),
    });
  }
  onSelectedBank(){//kategori ekle seçilirse modalShow fonksiyonunu çalıştır
    if(this.selectCategory==="kategori ekle"){
      this.selectCategory=null
     this.modalShow()
    }
  }
  add() {
    if (this.productAddForm.valid) {
      let productModel: Product = Object.assign({}, this.productAddForm.value);
      productModel.categoryId = Number(productModel.categoryId);
      console.log(productModel);
      this.productService.addProducts(productModel).subscribe(
        (data) => {
          this.toastrService.success(data.message, 'Başarılı');
        },
        (dataError) => {
          console.log(dataError);
          this.toastrService.error(dataError.error.message, 'Hata');
        }
      );
    } else {
      this.toastrService.error('Formunuz eksik', 'Dikkat');
    }
  }
  modalShow(){
   $("#productModel").modal('hide')//productModalı gizle
   $("#categoryAddModal").modal('show') //categoryAddModal ı göster
  }
}
