using System;
using System.Collections.Generic;
using System.Linq;
using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.ValidationRules.FluentValidations;
using Core.Aspects.Caching;
using Core.Aspects.Validation;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class OrderManager : IOrderService
    {
        private IOrderDal _orderDal;
        private IOrderDetailService _orderDetailService;
        private IProductService _productService;
        private IWalletService _walletService;
        private IUserService _userService;

        public OrderManager(IOrderDal orderDal, IProductService productService, IWalletService walletService, IOrderDetailService orderDetailService, IUserService userService)
        {
            _orderDal = orderDal;
            _productService = productService;
            _walletService = walletService;
            _orderDetailService = orderDetailService;
            _userService = userService;
        }

        [SecuredOperation("kullanıcı")]
        [ValidationAspect(typeof(OrderValidator))]
        [CacheRemoveAspect("IOrderService.Get")]
        public IResult Update(Order order, List<Product> products)//want servisden sipariş ve uygun ürünlerin listesi geliyor
        {
            var customerWallet = _walletService.GetByUserId(order.CustomerId).Data;//müşteri cüzdanı
            decimal quantity = order.Quantity;
            if (customerWallet.Amount >= fiyatartiYuzdebir(order.Quantity * order.UnitPrice))//ürün + muhasebeci payına parası yetiyor mu
            {
                var orderDetails = OrderControl(ref order, products, customerWallet);//order control methodundan detayları çekiyor
                order.OrderPending = false;//order gerçekleşti
                _orderDal.Update(order);//güncelleniyor

                if (quantity != order.Quantity)//başta istenen miktar verilen miktara eşit değilse
                {
                    var newOrder = order;//eskisinden yeni nesne oluşuyor
                    newOrder.Quantity = quantity - order.Quantity;//miktarı belirleniyor eksik kalan siparişin
                    newOrder.OrderPending = true;//beklemeye alınıyor
                    newOrder.Id = 0;//id =0 yeni veri
                    _orderDal.Add(newOrder);//tabloya ekleniyor
                }
                _orderDetailService.AddList(orderDetails.Data);//detaylar ekleniyor
                return new SuccessResult();
            }

            return new ErrorResult();
        }
        [SecuredOperation("kullanıcı")]
        [ValidationAspect(typeof(OrderValidator))]
        [CacheRemoveAspect("IOrderService.Get")]
        public IResult Add(Order order)
        {
            var quantity = order.Quantity;
            var result = _productService.GetByCategoryId(order.CategoryId).Data.OrderBy(p => p.UnitPrice).ToList();//ürünler fiyata göre küçükten büyüğe listeleniyor
            result =result.Where(p => p.SupplierId != order.CustomerId).ToList();
            if (order.UnitPrice>0)//fiyat sıfırdan büyükse istenen fiyattaki ürünler seçiliyor değilse eski sistemdeki gibi otomatik ucuz ürün ataması
            {
                result = result.Where(p => p.UnitPrice == order.UnitPrice).ToList();
            }
            var customerWallet = _walletService.GetByUserId(order.CustomerId).Data;//müşteri cüzdanı
            var orderDetails = OrderControl(ref order, result, customerWallet);//order controlden detaylar çekiliyor
            if (orderDetails.Success)//işlem başarılı ise
            {

                order.OrderDate = DateTime.Now;//tarih ataması
                order.OrderPending = false;//gerçekleşti
                _orderDal.Add(order);//veritabanına eklenme

                if (quantity != order.Quantity)//verilen miktar eksikse update deki olaylar
                {
                    var newOrder = order;
                    newOrder.Quantity = quantity - order.Quantity;
                    newOrder.OrderPending = true;
                    newOrder.Id = 0;
                    _orderDal.Add(newOrder);
                }
                foreach (var item in orderDetails.Data)//id ataması
                {
                    item.OrderId = order.Id;
                }
                _orderDetailService.AddList(orderDetails.Data);//db ye eklenme
                return new SuccessResult();
            }
            else//başarısız ise bekliyor olarak eklenmesi
            {
                order.OrderDate = DateTime.Now;
                order.OrderPending = true;
                _orderDal.Add(order);
            }
            return new ErrorResult(message:orderDetails.Message);
        }

        private IDataResult<List<OrderDetail>> OrderControl(ref Order order, List<Product> products, Wallet customerWallet)
        {
            var orderquantity = order.Quantity; //bu kadar ürün almak istiyorum 
            List<OrderDetail> orderDetails = new List<OrderDetail>();

            foreach (var product in products)
            {
                var canbuy = product.Quantity > orderquantity ? (orderquantity) : (product.Quantity); //alabilceğim ürün
                var purchased = customerWallet.Amount >= fiyatartiYuzdebir(canbuy * product.UnitPrice)
                    ? canbuy
                    : (customerWallet.Amount / fiyatartiYuzdebir(product.UnitPrice)); //verilen ürün
                orderquantity -= purchased;

                if (purchased > 0)//ürün verebiliyorsak
                {
                    var fiyat = purchased * product.UnitPrice;
                    var supplierWallet = _walletService.GetByUserId(product.SupplierId).Data;
                    var muhasabeci = _userService.GetMuhasabeci().Data;
                    var muhasabeciWallet = _walletService.GetByUserId(muhasabeci.Id).Data;
                    customerWallet.Amount -= fiyatartiYuzdebir(fiyat);//müşteriden muhasebeci payı + fiyat düşmesi
                    muhasabeciWallet.Amount += fiyat / 100;//muhasebeciye eklenmesi
                    supplierWallet.Amount += fiyat;//satıcıya eklenmesi
                    _walletService.Update(supplierWallet);
                    _walletService.Update(customerWallet);
                    _walletService.Update(muhasabeciWallet);
                    product.Quantity -= purchased;//ürünün güncellenmesi
                    _productService.Update(product);
                    orderDetails.Add(new OrderDetail { OrderId = order.Id, ProductId = product.Id, Quantity = purchased, OrderDate = DateTime.Now });//detayın eklenmesi
                }

                if (orderquantity == 0)
                {
                    break;
                }
            }

            if (orderquantity != order.Quantity)
            {
                order.Quantity -= orderquantity;
                return new SuccessDataResult<List<OrderDetail>>(orderDetails);
            }

            return new ErrorDataResult<List<OrderDetail>>(message:"istek beklemede");
        }
        public IDataResult<List<Order>> GetByCategoryIdPendingOrders(int categoryId)//kategori id sine göre beklemedeki siparişlerin listelenmesi
        {
            var result = _orderDal.GetAll(o => o.OrderPending == true && o.CategoryId == categoryId);
            return new SuccessDataResult<List<Order>>(result);
        }

        public IDataResult<List<Order>> GetByReport(ReportInfo reportInfo)//report listesinin oluşması
        {
            var result = _orderDal.GetAll(o=>o.CustomerId==reportInfo.UserId
            &&o.OrderPending==false&&o.OrderDate>=reportInfo.StartTime&&o.OrderDate<=reportInfo.EndTime);
            return new SuccessDataResult<List<Order>>(result);
        }

        decimal fiyatartiYuzdebir(decimal fiyat)
        {
            decimal yuzdebir = fiyat / 100;
            return fiyat + yuzdebir;
        }
    }
}