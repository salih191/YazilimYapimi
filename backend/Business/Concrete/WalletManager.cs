using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class WalletManager : IWalletService
    {
        private IWalletDal _walletDal;

        public WalletManager(IWalletDal walletDal)
        {
            _walletDal = walletDal;
        }

        [SecuredOperation("kullanıcı")]
        public IResult Update(Wallet wallet)
        {
            _walletDal.Update(wallet);
            return new SuccessResult();
        }


        [SecuredOperation("kullanıcı")]
        public IDataResult<Wallet> GetByUserId(int userId)
        {
            return new SuccessDataResult<Wallet>(_walletDal.Get(w => w.UserId == userId));
        }
    }
}