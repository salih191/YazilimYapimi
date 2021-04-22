using System.Collections.Generic;
using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.ValidationRules.FluentValidations;
using Core.Aspects.Caching;
using Core.Aspects.Validation;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;

namespace Business.Concrete
{
    public class AddMoneyManager : IAddMoneyService
    {
        private IAddMoneyDal _addMoneyDal;

        public AddMoneyManager(IAddMoneyDal addMoneyDal)
        {
            _addMoneyDal = addMoneyDal;
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
        public IResult Confirm(int id)//id ile onaylama 
        {
            var result = _addMoneyDal.Get(a => a.Id == id);//id üzerinden bilgiler çekiliyor
            result.Status = true;//durum onaylandı yapılıyor
            _addMoneyDal.Update(result);//veritabanına güncelleniyor veritabanındaki addMOney trigger ile işlem yapılıyor
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