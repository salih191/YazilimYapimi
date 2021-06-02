using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.ValidationRules.FluentValidations;
using Core.Aspects.Caching;
using Core.Aspects.Validation;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;

namespace Business.Concrete
{
    public class AddMoneyManager : IAddMoneyService
    {
        private IAddMoneyDal _addMoneyDal;
        private IWalletService _walletService;

        public AddMoneyManager(IAddMoneyDal addMoneyDal, IWalletService walletService)
        {
            _addMoneyDal = addMoneyDal;
            _walletService = walletService;
        }

        [SecuredOperation("kullanıcı")]//kullanıcı yetkisi olanlar erişebilir
        [ValidationAspect(typeof(AddMoneyValidator))]//validasyon
        [CacheRemoveAspect("IAddMoneyService.Get")]//cach siliniyor
        public IResult Add(AddMoney money)//addMoney tablosunda adminin beklemesine gidiyor
        {
            money.Status = false;
            _addMoneyDal.Add(money);
            return new SuccessResult("Para Admin Onayına İletildi");
        }

        [SecuredOperation("")]
        [CacheAspect]
        public IDataResult<List<AddMoneyDto>> GetApproved()//onaylanacaklar çekiliyor
        {
            return new SuccessDataResult<List<AddMoneyDto>>(_addMoneyDal.GetAddMoneyDtos());
        }

        [SecuredOperation("")]
        [CacheRemoveAspect("IAddMoneyService.Get")]
        public async Task<IResult> Confirm(AddMoneyDto addMoneyDto)//id ile onaylama 
        {
            string adres = "https://api.genelpara.com/embed/doviz.json";
            var myType = MyTypeBuilder.CompileResultType(new List<Field>() { new Field { FieldName = addMoneyDto.CurrencyType, FieldType = typeof(Money) } });
            var data = await WebApiHelper.GetMethod(adres, myType);
            Money money=data.GetType().GetProperty(addMoneyDto.CurrencyType).GetValue(data) as Money;
            var result = _addMoneyDal.Get(a => a.Id == addMoneyDto.AddMoneyId);//id üzerinden bilgiler çekiliyor
            if (result.Status)
            {
                return new ErrorResult();
            }
            result.Status = true;//durum onaylandı yapılıyor
            _addMoneyDal.Update(result);
            var dovizKuru=money == null ? "1":money.Satis;
            decimal doviz = Convert.ToDecimal(dovizKuru);
            _walletService.AddMoney(new Wallet {Amount = addMoneyDto.Amount*doviz, UserId = result.UserId});
            return new SuccessResult("onaylandı");
        }

        [SecuredOperation("")]
        [CacheRemoveAspect("IAddMoneyService.Get")]
        public IResult Reject(int addMoneyId)//reddetme
        {
            _addMoneyDal.Delete(new AddMoney { Id = addMoneyId });//tablodan siliniyor veri
            return new SuccessResult("reddedildi");
        }
    }
}