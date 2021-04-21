using System.Collections.Generic;
using Business.Abstract;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;

namespace Business.Concrete
{
    public class OrderDetailManager : IOrderDetailService
    {
        private IOrderDetailDal _orderDetailDal;

        public OrderDetailManager(IOrderDetailDal orderDetailDal)
        {
            _orderDetailDal = orderDetailDal;
        }

        public IResult AddList(List<OrderDetail> orderDetails)
        {
            _orderDetailDal.AddList(orderDetails);
            return new SuccessResult();
        }

        public IDataResult<List<OrderDetailsDto>> GetOrderDetailsDtos()
        {
            return new SuccessDataResult<List<OrderDetailsDto>>(_orderDetailDal.getAllDetailsDtos());
        }
    }
}