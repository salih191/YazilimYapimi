using System.Collections.Generic;
using Business.Abstract;
using Core.Utilities.Results;
using Entities.Concrete;

namespace Business.Concrete
{
    public class WantManager:IWantService
    {
        private IOrderService _orderService;
        private IProductService _productService;

        public WantManager(IOrderService orderService, IProductService productService)
        {
            _orderService = orderService;
            _productService = productService;
        }

        public IResult Want(Product product)
        {
            var result = _orderService.GetByCategoryIdPendingOrders(product.CategoryId).Data;//beklemedeki siparişler
            var products = _productService.GetByCategoryId(product.CategoryId).Data;//uyan ürünler
            var matchingProducts = new List<Product>();//eşleşen ürünler
            foreach (var order in result)
            {
                decimal quantity = 0;
                foreach (var p in products)
                {
                    if (order.CustomerId != p.SupplierId && p.UnitPrice == order.UnitPrice)//kendi ürünü olmayan ve fiyatı eşleşen
                    {
                        quantity += p.Quantity;
                        matchingProducts.Add(p);
                        if (quantity > 0)
                        {
                            _orderService.Update(order, matchingProducts);
                        }
                    }
                }
            }

            return new SuccessResult();
        }
    }
}