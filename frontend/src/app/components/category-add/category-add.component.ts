import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { CategoryService } from 'src/app/services/category.service';

@Component({
  selector: 'app-category-add',
  templateUrl: './category-add.component.html',
  styleUrls: ['./category-add.component.css']
})
export class CategoryAddComponent implements OnInit {

  categoryAddForm: FormGroup;
  constructor(
    private formBuilder: FormBuilder,
    private categoryService: CategoryService,
    private toastrService: ToastrService
  ) {}

  ngOnInit(): void {
    this.createCategoryAddForm();
  }

  createCategoryAddForm() {
    this.categoryAddForm = this.formBuilder.group({
      name: ['', Validators.required],
    });
  }
  add() {
    if (this.categoryAddForm.valid) {//kategori ekleme formu validasyona uyuyor mu
      let categoryModel = Object.assign({}, this.categoryAddForm.value);//api ye gönderilecek nesne oluşuyor formdan girilen bilgilerle
      this.categoryService.addCategory(categoryModel).subscribe(//servise category gönderiliyor
        (data) => {//olumlu sonuç gelirse
          this.toastrService.success(data.message, 'Başarılı');//mesaj
          window.history.go(0)//sayfayı yenile kategorinin gelmesi için
        },
        (dataError) => {//badrequest dönerse
              this.toastrService.error(dataError.error, 'Hata');//mesaj
        }
      );
    } else {
      this.toastrService.error('Formunuz eksik', 'Dikkat');//mesaj
    }
  }
}
