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
        public IResult OldAdd(Order order)
        {
            var result = _productService.GetByCategoryId(order.CategoryId).Data.OrderBy(p => p.UnitPrice).ToList();//ürünler fiyata göre küçükten büyüğe listeleniyor
            result = result.Where(p => p.SupplierId != order.CustomerId).ToList();//satıcının kendi ürünleri çıkarılıyor listeden
            var quantity = order.Quantity;//istediği ürün miktarı
            decimal sumAddQuantity = 0;//ne kadar ürün verdik
            var customerWallet = _walletService.GetByUserId(order.CustomerId).Data;//alıcı cüzdan bilgileri çekiliyor
            List<OrderDetail> orderDetails = new List<OrderDetail>();//oluşucak sipariş detaylarını tutacak liste
            foreach (var product in result)
            {
                var supplierWallet = _walletService.GetByUserId(product.SupplierId).Data;//satıcı cüzdanı
                var canAfford = customerWallet.Amount / product.UnitPrice;//alıcının parası kaç ürüne yetiyor
                var canTake = canAfford > product.Quantity ? product.Quantity : canAfford;//bu üründen ne kadar alabilir
                var addQuantity = canTake > (quantity - sumAddQuantity) ? (quantity - sumAddQuantity) : canTake;//alınan ürün miktarı

                if (addQuantity > 0)//ürün verildiyse
                {
                    supplierWallet.Amount += addQuantity * product.UnitPrice;//satıcıya para eklenmesi
                    customerWallet.Amount -= addQuantity * product.UnitPrice;//alıcıdan para çıkması
                    sumAddQuantity += addQuantity;//verilen miktar tüm verilene ekleniyor
                    _walletService.Update(supplierWallet);//cüzdan güncelleniyor
                    _walletService.Update(customerWallet);//cüzdan güncelleniyor
                    product.Quantity -= addQuantity;//verilen ürün miktarı düşülüyor
                    _productService.Update(product);//ürün güncelleniyor
                    orderDetails.Add(new OrderDetail { ProductId = product.Id, Quantity = addQuantity });//detay oluşuyor
                }
                if (quantity == sumAddQuantity)//istediği kadar verdiysek döngü sonlanıyor
                {
                    break;
                }
            }
            if (sumAddQuantity > 0)//ürün verildiyse order oluşuyor
            {
                order.Quantity = sumAddQuantity;
                order.OrderDate = DateTime.Now;
                _orderDal.Add(order);
                foreach (var orderDetail in orderDetails)//order detailse order id ekleniyor
                {
                    orderDetail.OrderId = order.Id;
                }
                _orderDetailService.AddList(orderDetails);//order details liste olarak ekleniyor
                string mesaj = sumAddQuantity + " ürün satın alımı gerçekleşti." + ((sumAddQuantity < quantity) ? " İşlem eksik tamamlandı" : "");
                return new SuccessResult(mesaj);
            }
            return new ErrorResult("satın alım gerçekleşemedi");
        }

        public IResult Update(Order order, List<Product> products)
        {
            var customerWallet = _walletService.GetByUserId(order.CustomerId).Data;
            decimal quantity = order.Quantity;
            if (customerWallet.Amount >= order.Quantity * order.UnitPrice)
            {
                var orderDetails = OrderControl(ref order, products, customerWallet);
                order.OrderPending = false;
                _orderDal.Update(order);

                if (quantity != order.Quantity)
                {
                    var newOrder = order;
                    newOrder.Quantity = quantity - order.Quantity;
                    newOrder.OrderPending = true;
                    newOrder.Id = 0;
                    _orderDal.Add(newOrder);
                }
                _orderDetailService.AddList(orderDetails.Data);
                return new SuccessResult();
            }

            return new ErrorResult();
        }

        public IResult Add(Order order)
        {
            var quantity = order.Quantity;
            var result = _productService.GetByCategoryId(order.CategoryId).Data.OrderBy(p => p.UnitPrice).ToList();//ürünler fiyata göre küçükten büyüğe listeleniyor
            result = result.Where(p => p.SupplierId != order.CustomerId && p.UnitPrice == order.UnitPrice).ToList();
            var customerWallet = _walletService.GetByUserId(order.CustomerId).Data;
            var orderDetails = OrderControl(ref order, result, customerWallet);
            if (orderDetails.Success)
            {

                order.OrderDate = DateTime.Now;
                order.OrderPending = false;
                _orderDal.Add(order);

                if (quantity != order.Quantity)
                {
                    var newOrder = order;
                    newOrder.Quantity = quantity - order.Quantity;
                    newOrder.OrderPending = true;
                    newOrder.Id = 0;
                    _orderDal.Add(newOrder);
                }
                foreach (var item in orderDetails.Data)
                {
                    item.OrderId = order.Id;
                }
                _orderDetailService.AddList(orderDetails.Data);
                return new SuccessResult();
            }
            else
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

                if (purchased > 0)
                {
                    var fiyat = purchased * product.UnitPrice;
                    var supplierWallet = _walletService.GetByUserId(product.SupplierId).Data;
                    var muhasabeci = _userService.GetMuhasabeci().Data;
                    var muhasabeciWallet = _walletService.GetByUserId(muhasabeci.Id).Data;
                    customerWallet.Amount -= fiyatartiYuzdebir(fiyat);
                    muhasabeciWallet.Amount += fiyat / 100;
                    supplierWallet.Amount += fiyat;
                    _walletService.Update(supplierWallet);
                    _walletService.Update(customerWallet);
                    _walletService.Update(muhasabeciWallet);
                    product.Quantity -= purchased;
                    _productService.Update(product);
                    orderDetails.Add(new OrderDetail { OrderId = order.Id, ProductId = product.Id, Quantity = purchased, OrderDate = DateTime.Now });
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
        public IDataResult<List<Order>> GetByCategoryIdPendingOrders(int categoryId)
        {
            var result = _orderDal.GetAll(o => o.OrderPending == true && o.CategoryId == categoryId);
            return new SuccessDataResult<List<Order>>(result);
        }

        decimal fiyatartiYuzdebir(decimal fiyat)
        {
            decimal yuzdebir = fiyat / 100;
            return fiyat + yuzdebir;
        }
    }
}