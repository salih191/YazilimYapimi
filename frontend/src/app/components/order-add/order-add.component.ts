import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { Category } from 'src/app/models/category/category';
import { Order } from 'src/app/models/order/order';
import { AuthService } from 'src/app/services/auth.service';
import { CategoryService } from 'src/app/services/category.service';
import { OrderService } from 'src/app/services/order.service';

@Component({
  selector: 'app-order-add',
  templateUrl: './order-add.component.html',
  styleUrls: ['./order-add.component.css'],
})
export class OrderAddComponent implements OnInit {
  orderAddForm: FormGroup;
  categories: Category[];
  constructor(
    private formBuilder: FormBuilder,
    private orderService: OrderService,
    private toastrService: ToastrService,
    private authService: AuthService,
    private categoryService: CategoryService
  ) {}

  ngOnInit(): void {
    this.createOrderAddForm();
    this.categoryService.getCategory().subscribe(
      (response) => {
        this.categories = response.data;
      },
      (errorResponse) => {
        console.log(errorResponse);
      }
    );
  }

  createOrderAddForm() {
    this.orderAddForm = this.formBuilder.group({
      categoryId: ['', Validators.required],
      quantity: ['', Validators.required],
      customerId: Number(this.authService.getUser().id),
    });
  }

  add() {
    if (this.orderAddForm.valid) {
      let orderModel: Order = Object.assign({}, this.orderAddForm.value);
      orderModel.categoryId = Number(orderModel.categoryId);
      this.orderService.addOrders(orderModel).subscribe(
        (data) => {
          this.toastrService.success(data.message, 'Başarılı');
          window.history.replaceState({}, '', '');
          window.history.go(0);
        },
        (dataError) => {
          this.toastrService.error(dataError.error.message, 'Hata');
        }
      );
    } else {
      this.toastrService.error('Formunuz eksik', 'Dikkat');
    }
  }
}
