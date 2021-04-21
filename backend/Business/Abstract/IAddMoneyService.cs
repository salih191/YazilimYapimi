using System.Collections.Generic;
using Core.Utilities.Results;
using Entities.Concrete;
using Entities.DTOs;

namespace Business.Abstract
{
    public interface IAddMoneyService
    {
        IResult Add(AddMoney money);
        IDataResult<List<AddMoneyDto>> GetApproved();
        IResult Confirm(int addMoneyId);
        IResult Reject(int addMoneyId);
    }
}