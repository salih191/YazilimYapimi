using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Utilities.Results;
using Entities.Concrete;
using Entities.DTOs;

namespace Business.Abstract
{
    public interface IAddMoneyService
    {
        IResult Add(AddMoney money);
        Task<IResult> Confirm(AddMoneyDto addMoneyDto);
        IDataResult<List<AddMoneyDto>> GetApproved();
        IResult Reject(int addMoneyId);
    }
}