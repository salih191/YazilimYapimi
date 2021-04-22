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

        public OrderManager(IOrderDal orderDal, IProductService productService, IWalletService walletService, IOrderDetailService orderDetailService)
        {
            _orderDal = orderDal;
            _productService = productService;
            _walletService = walletService;
            _orderDetailService = orderDetailService;
        }

        [SecuredOperation("kullanıcı")]
        [ValidationAspect(typeof(OrderValidator))]
        [CacheRemoveAspect("IOrderService.Get")]
        public IResult Add(Order order)
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
    }
}