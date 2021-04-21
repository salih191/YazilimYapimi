using System.Collections.Generic;
using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Core.Aspects.Caching;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        private IUserDal _userDal;

        public UserManager(IUserDal userDal)
        {
            _userDal = userDal;
        }

        public IResult Add(User entity)
        {
            _userDal.Add(entity);
            _userDal.AddUserOperationClaim(new UserOperationClaim { OperationClaimId = 2, UserId = entity.Id });
            return new SuccessResult();
        }


        [SecuredOperation("kullanıcı")]
        public IDataResult<User> GetById(int id)
        {
            return new SuccessDataResult<User>(_userDal.Get(u => u.Id == id));
        }


        [SecuredOperation("kullanıcı")]
        [CacheRemoveAspect("IUserService.Get")]
        public IResult Update(User entity)
        {
            _userDal.Update(entity);
            return new SuccessResult();
        }

        public IDataResult<List<OperationClaim>> GetClaims(User user)
        {
            return new SuccessDataResult<List<OperationClaim>>(data: _userDal.GetClaims(user));
        }

        public IDataResult<User> GetByMail(string email)
        {
            return new SuccessDataResult<User>(data: _userDal.Get(u => u.Email.ToLower() == email.ToLower()));
        }
        public IDataResult<User> GetByUserName(string userName)
        {
            return new SuccessDataResult<User>(_userDal.Get(u => u.UserName == userName));
        }
    }
}