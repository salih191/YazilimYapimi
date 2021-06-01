using System.Collections.Generic;
using Core.Utilities.Results;
using Entities.Concrete;
using Entities.DTOs;

namespace Business.Abstract
{
    public interface IAddMoneyService
    {
        IResult Add(AddMoney money);
        IResult Confirm(AddMoneyDto addMoneyDto);
        IDataResult<List<AddMoneyDto>> GetApproved();
        IResult Reject(int addMoneyId);
    }
}