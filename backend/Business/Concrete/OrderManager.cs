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
            var result = _productService.GetByCategoryId(order.CategoryId).Data.OrderBy(p => p.UnitPrice).ToList();
            result = result.Where(p => p.SupplierId != order.CustomerId).ToList();
            var quantity = order.Quantity;
            decimal sumAddQuantity = 0;
            var customerWallet = _walletService.GetByUserId(order.CustomerId).Data;
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            foreach (var product in result)
            {
                var supplierWallet = _walletService.GetByUserId(product.SupplierId).Data;
                var canAfford = customerWallet.Amount / product.UnitPrice;//alıcının parası kaç ürüne yetiyor
                var canTake = canAfford > product.Quantity ? product.Quantity : canAfford;//bu üründen ne kadar alabilir
                var addQuantity = canTake > (quantity - sumAddQuantity) ? (quantity - sumAddQuantity) : canTake;//alınan ürün miktarı
                supplierWallet.Amount += addQuantity * product.UnitPrice;//satıcıya para eklenmesi
                customerWallet.Amount -= addQuantity * product.UnitPrice;//alıcıdan para çıkması
                sumAddQuantity += addQuantity;
                _walletService.Update(supplierWallet);
                _walletService.Update(customerWallet);
                product.Quantity -= addQuantity;//product quantity eksilmesi
                _productService.Update(product);
                if (addQuantity > 0)
                {
                    orderDetails.Add(new OrderDetail { ProductId = product.Id, Quantity = addQuantity });
                }
                if (quantity == sumAddQuantity)
                {
                    break;
                }
            }
            if (sumAddQuantity > 0)
            {
                order.Quantity = sumAddQuantity;
                order.OrderDate = DateTime.Now;
                _orderDal.Add(order);
                foreach (var orderDetail in orderDetails)
                {
                    orderDetail.OrderId = order.Id;
                }
                _orderDetailService.AddList(orderDetails);
                string mesaj = sumAddQuantity + " ürün satın alımı gerçekleşti." + ((sumAddQuantity < quantity) ? " İşlem eksik tamamlandı" : "");
                return new SuccessResult(mesaj);
            }
            return new ErrorResult("satın alım gerçekleşemedi");
        }
    }
}