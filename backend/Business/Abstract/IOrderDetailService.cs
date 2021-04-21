using System.Collections.Generic;
using Core.Utilities.Results;
using Entities.Concrete;
using Entities.DTOs;

namespace Business.Abstract
{
    public interface IOrderDetailService
    {
        IResult AddList(List<OrderDetail> orderDetails);
        IDataResult<List<OrderDetailsDto>> GetOrderDetailsDtos();
    }
}