using System.Collections.Generic;
using System.Linq;
using Core.DataAccess.EntityFramework;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;

namespace DataAccess.Concrete
{
    public class EfAddProductDal : EfEntityRepositoryBase<AddProduct, YazilimYapimiContext>, IAddProductDal
    {
        public List<AddProductDto> GetAddMoneyDtos()
        {
            using (var context = new YazilimYapimiContext())
            {
                var result = from addProduct in context.AddProducts
                    join user in context.Users on addProduct.SupplierId equals user.Id
                    join category in context.Categories on addProduct.CategoryId equals category.Id
                    where addProduct.Status == false
                    select new AddProductDto
                    {
                        AddProductId = addProduct.Id,
                        CategoryName = category.Name,
                        Quantity = addProduct.Quantity,
                        SupplierUserName = user.UserName,
                        UnitsInPrice = addProduct.UnitPrice
                    };
                return result.ToList();
            }
        }
    }
}