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

        [SecuredOperation("kullanıcı")]
        [ValidationAspect(typeof(AddMoneyValidator))]
        [CacheRemoveAspect("IAddMoneyService.Get")]
        public IResult Add(AddMoney money)
        {
            _addMoneyDal.Add(money);
            return new SuccessResult("Para Admin Onayına İletildi");
        }

        [SecuredOperation("")]
        [CacheAspect]
        public IDataResult<List<AddMoneyDto>> GetApproved()
        {
            return new SuccessDataResult<List<AddMoneyDto>>(_addMoneyDal.GetAddMoneyDtos());
        }

        [SecuredOperation("")]
        [CacheRemoveAspect("IAddMoneyService.Get")]
        public IResult Confirm(int id)
        {
            var result = _addMoneyDal.Get(a => a.Id == id);
            result.Status = true;
            _addMoneyDal.Update(result);
            return new SuccessResult("onaylandı");
        }

        [SecuredOperation("")]
        [CacheRemoveAspect("IAddMoneyService.Get")]
        public IResult Reject(int addMoneyId)
        {
            _addMoneyDal.Delete(new AddMoney { Id = addMoneyId });
            return new SuccessResult("reddedildi");
        }
    }
}