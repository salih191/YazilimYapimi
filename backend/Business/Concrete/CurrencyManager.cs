using System.Collections.Generic;
using Business.Abstract;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;

namespace Business.Concrete
{
    public class CurrencyManager:ICurrencyService
    {
        private ICurrencyDal _currencyDal;

        public CurrencyManager(ICurrencyDal currencyDal)
        {
            _currencyDal = currencyDal;
        }

        public IDataResult<List<Currency>> GetAllCurrency()
        {
            return new SuccessDataResult<List<Currency>>(_currencyDal.GetAll());
        }
    }
}