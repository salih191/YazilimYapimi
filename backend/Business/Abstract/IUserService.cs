using System.Collections.Generic;
using Core.Entities.Concrete;
using Core.Utilities.Results;

namespace Business.Abstract
{
    public interface IUserService
    {
        IDataResult<List<OperationClaim>> GetClaims(User user);
        IResult Add(User user);
        IDataResult<User> GetByMail(string email);
        IDataResult<User> GetByUserName(string userName);
        IResult Update(User user);
        IDataResult<User> GetById(int userId);
        IDataResult<User> GetMuhasabeci();
    }
}