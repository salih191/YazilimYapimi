using Core.Utilities.Results;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface IWalletService
    {
        IResult Update(Wallet wallet);
        IResult AddMoney(Wallet wallet);
        IDataResult<Wallet> GetByUserId(int userId);
    }
}