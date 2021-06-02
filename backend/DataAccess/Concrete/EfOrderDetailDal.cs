using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;

namespace DataAccess.Concrete
{
    public class EfOrderDetailDal : EfEntityRepositoryBase<OrderDetail, YazilimYapimiContext>, IOrderDetailDal
    {
        public List<OrderDetailsDto> getAllDetailsDtos(Expression<Func<OrderDetail, bool>> filter = null)
        {
            using (var context = new YazilimYapimiContext())
            {
                var result = from orderDetail in filter==null? context.OrderDetails:context.OrderDetails.Where(filter)
                    join Order in context.Orders on orderDetail.OrderId equals Order.Id
                    join product in context.Products on orderDetail.ProductId equals product.Id
                    join category in context.Categories on product.CategoryId equals category.Id
                    join customerUser in context.Users on Order.CustomerId equals customerUser.Id
                    join supplierUser in context.Users on product.SupplierId equals supplierUser.Id
                    select new OrderDetailsDto
                    {
                        OrderDetailId = orderDetail.Id,
                        Amount = product.UnitPrice * orderDetail.Quantity,
                        CustomerUserName = customerUser.UserName,
                        OrderDate = Order.OrderDate,
                        ProductCategory = category.Name,
                        Quantity = product.Quantity,
                        SupplierUserName = supplierUser.UserName
                    };
                return result.ToList();
            }
        }
    }
}