using System.Collections.Generic;
using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Core.Aspects.Caching;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.DTOs;

namespace Business.Concrete
{
    public class ProductManager : IProductService
    {
        private IProductDal _productDal;
        public ProductManager(IProductDal productDal)
        {
            _productDal = productDal;
        }

        [SecuredOperation("kullanıcı")]
        [CacheRemoveAspect("IProductService.Get")]
        public IResult Add(Product product)
        {
            _productDal.Add(product);
            return new SuccessResult();
        }

        [SecuredOperation("kullanıcı")]
        [CacheRemoveAspect("IProductService.Get")]
        public IResult Update(Product product)
        {
            _productDal.Update(product);
            return new SuccessResult();
        }
        public IDataResult<Product> IsThereAnyProduct(AddProduct addProduct)
        {
            return new SuccessDataResult<Product>(_productDal.Get(p => 
                p.CategoryId == addProduct.CategoryId &&
                p.SupplierId == addProduct.SupplierId&&
                p.UnitPrice==addProduct.UnitPrice)
            );
        }

        [SecuredOperation("kullanıcı")]
        [CacheAspect()]
        public IDataResult<List<Product>> GetByCategoryId(int categoryId)
        {
            return new SuccessDataResult<List<Product>>(_productDal.GetAll(p => p.CategoryId == categoryId));
        }

        [CacheAspect()]
        public IDataResult<List<ProductDto>> GetAllProductsDetail()
        {
            return new SuccessDataResult<List<ProductDto>>(_productDal.GetProductsDetail());
        }
    }
}