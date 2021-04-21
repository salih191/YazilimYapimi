using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.JWT;
using Entities.DTOs;

namespace Business.Abstract
{
    public interface IAuthService
    {
        IDataResult<User> Register(UserForRegisterDto userForRegisterDto, string password);
        IDataResult<User> Login(UserForLoginDto userForLoginDto);
        IResult UserExists(string email, string userName);
        IDataResult<AccessToken> CreateAccessToken(IDataResult<User> dataResult);
        IResult ChangePassword(UserForChangePasswordDto userForChangePasswordDto);
    }
}