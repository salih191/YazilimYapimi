using System.Collections.Generic;
using Core.Utilities.Results;
using Entities.Concrete;
using Entities.DTOs;

namespace Business.Abstract
{
    public interface IProductService
    {
        IResult Update(Product product);
        IDataResult<List<Product>> GetByCategoryId(int categoryId);
        IDataResult<List<Product>> GetAll();
        IDataResult<List<ProductDto>> GetAllProductsDetail();
    }
}