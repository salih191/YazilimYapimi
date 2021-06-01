using System.Collections.Generic;
using Core.Utilities.Results;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface IOrderService
    {
        IResult Add(Order order);
        IResult Update(Order order,List<Product> products);
        IDataResult<List<Order>> GetByCategoryIdPendingOrders(int categoryId);
    }
}