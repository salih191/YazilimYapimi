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
        private IProductService _productService;
        private IWantService _wantService;

        public AddProductManager(IAddProductDal addProductDal, IProductService productService, IWantService wantService)
        {
            _addProductDal = addProductDal;
            _productService = productService;
            _wantService = wantService;
        }

        [SecuredOperation("kullanıcı")]
        [ValidationAspect(typeof(AddProductValidator))]
        [CacheRemoveAspect("IAddProductService.Get")]
        public IResult Add(AddProduct product)//addproduct tablosunda adminin beklemesine gidiyor
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
        public IResult Confirm(AddProductDto addProductDto)
        {
            var result = _addProductDal.Get(a => a.Id == addProductDto.AddProductId);

            if (result.Status)
            {
                return new ErrorResult();
            }
            result.Status = true;
            var product = _productService.IsThereAnyProduct(result).Data;
            if (product != null)
            {
                product.Quantity += result.Quantity;
                _productService.Update(product);
                //_addProductDal.Update(result);
            }
            else
            {
                _productService.Add(new Product
                {
                    Quantity = result.Quantity,
                    UnitPrice = addProductDto.UnitsInPrice,
                    SupplierId = result.SupplierId,
                    CategoryId = result.CategoryId
                });
            }
            _wantService.Want(product);
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