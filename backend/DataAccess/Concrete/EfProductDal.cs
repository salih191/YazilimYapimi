using System.Collections.Generic;
using System.Linq;
using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;

namespace DataAccess.Concrete
{
    public class EfProductDal : EfEntityRepositoryBase<Product, YazilimYapimiContext>, IProductDal
    {
        public List<ProductDto> GetProductsDetail()
        {
            using (YazilimYapimiContext context = new YazilimYapimiContext())
            {
                var result = from product in context.Products
                    join Category in context.Categories on product.CategoryId equals Category.Id
                    join user in context.Users on product.SupplierId equals user.Id
                    select new ProductDto
                    {
                        ProductId = product.Id,
                        CategoryName = Category.Name,
                        Quantity = product.Quantity,
                        SupplierUserName = user.UserName,
                        UnitsInPrice = product.UnitPrice
                    };
                return result.ToList();
            }
        }
    }
}