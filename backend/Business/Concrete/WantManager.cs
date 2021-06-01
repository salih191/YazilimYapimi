﻿using System.Collections.Generic;
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
            var result = _orderService.GetByCategoryIdPendingOrders(product.CategoryId).Data;
            var products = _productService.GetByCategoryId(product.CategoryId).Data;
            var matchingProducts = new List<Product>();
            foreach (var order in result)
            {
                decimal quantity = 0;
                foreach (var p in products)
                {
                    if (order.CustomerId != p.SupplierId && p.UnitPrice == order.UnitPrice)
                    {
                        quantity += p.Quantity;
                        matchingProducts.Add(p);
                        if (quantity >= order.Quantity)
                        {
                            _orderService.Update(order, matchingProducts);
                            return new SuccessResult();
                        }
                    }
                }

                if (quantity>0)
                {
                    _orderService.Update(order, matchingProducts);
                    return new SuccessResult();
                }
            }

            return new ErrorResult();
        }
    }
}