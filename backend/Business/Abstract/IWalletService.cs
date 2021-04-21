using Core.Utilities.Results;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface IWalletService
    {
        IResult Update(Wallet wallet);
        IDataResult<Wallet> GetByUserId(int userId);
    }
}