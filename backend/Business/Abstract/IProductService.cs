using System.Collections.Generic;
using Core.Utilities.Results;
using Entities.Concrete;
using Entities.DTOs;

namespace Business.Abstract
{
    public interface IProductService
    {
        IResult Add(Product product);
        IResult Update(Product product);
        IDataResult<List<Product>> GetByCategoryId(int categoryId);
        IDataResult<List<ProductDto>> GetAllProductsDetail();
        IDataResult<Product> IsThereAnyProduct(AddProduct addProduct);
    }
}