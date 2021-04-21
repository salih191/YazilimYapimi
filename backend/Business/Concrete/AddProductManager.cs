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
    public class AddProductManager : IAddProductService
    {
        private IAddProductDal _addProductDal;

        public AddProductManager(IAddProductDal addProductDal)
        {
            _addProductDal = addProductDal;
        }

        [SecuredOperation("kullanıcı")]
        [ValidationAspect(typeof(AddProductValidator))]
        [CacheRemoveAspect("IAddProductService.Get")]
        public IResult Add(AddProduct product)
        {
            product.Status = false;
            _addProductDal.Add(product);
            return new SuccessResult("Ürün admin onayına iletildi");
        }


        [SecuredOperation("")]
        [CacheAspect]
        public IDataResult<List<AddProductDto>> GetApproved()
        {
            return new SuccessDataResult<List<AddProductDto>>(_addProductDal.GetAddMoneyDtos());
        }

        [SecuredOperation("")]
        [CacheRemoveAspect("IProductService.Get")]
        [CacheRemoveAspect("IAddProductService.Get")]
        public IResult Confirm(int addProductId)
        {
            var result = _addProductDal.Get(a => a.Id == addProductId);
            result.Status = true;
            _addProductDal.Update(result);
            return new SuccessResult("onaylandı");
        }


        [SecuredOperation("")]
        [CacheRemoveAspect("IAddProductService.Get")]
        public IResult Reject(int addProductId)
        {
            _addProductDal.Delete(new AddProduct { Id = addProductId });
            return new SuccessResult("reddedildi");
        }
    }
}