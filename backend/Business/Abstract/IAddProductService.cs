using System.Collections.Generic;
using Core.Utilities.Results;
using Entities.Concrete;
using Entities.DTOs;

namespace Business.Abstract
{
    public interface IAddProductService
    {
        IResult Add(AddProduct product);
        IDataResult<List<AddProductDto>> GetApproved();
        IResult Confirm(int addProductId);
        IResult Reject(int addProductId);
    }
}