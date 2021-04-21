using Business.Abstract;
using Business.ValidationRules.FluentValidations;
using Core.Aspects.Validation;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.JWT;
using Entities.DTOs;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private IUserService _userService;
        private ITokenHelper _tokenHelper;

        public AuthManager(IUserService userService, ITokenHelper tokenHelper)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
        }

        [ValidationAspect(typeof(UserForRegisterDtoValidator))]
        public IDataResult<User> Register(UserForRegisterDto userForRegisterDto, string password)
        {
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);
            var user = new User
            {
                Email = userForRegisterDto.Email,
                FirstName = userForRegisterDto.FirstName,
                LastName = userForRegisterDto.LastName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                UserName = userForRegisterDto.UserName,
                TCIdentityNumber = userForRegisterDto.TCIdentityNumber,
                PhoneNumber = userForRegisterDto.PhoneNumber,
                Address = userForRegisterDto.Address
            };
            _userService.Add(user);
            return new SuccessDataResult<User>(user, "Kayıt oldu");
        }

        public IResult ChangePassword(UserForChangePasswordDto userForChangePasswordDto)
        {
            var userToCheck = _userService.GetByMail(userForChangePasswordDto.Email).Data;
            if (!HashingHelper.VerifyPasswordHash(userForChangePasswordDto.OldPassword, userToCheck.PasswordHash, userToCheck.PasswordSalt))
            {
                return new ErrorDataResult<User>("Mevcut parola hatalı");
            }
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(userForChangePasswordDto.NewPassword, out passwordHash, out passwordSalt);
            var user = _userService.GetById(userForChangePasswordDto.UserId).Data;
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            _userService.Update(user);
            return new SuccessResult("Şifre değiştirildi");
        }
        public IDataResult<User> Login(UserForLoginDto userForLoginDto)
        {
            var userToCheck = _userService.GetByMail(userForLoginDto.Email).Data;
            if (userToCheck == null)
            {
                userToCheck = _userService.GetByUserName(userForLoginDto.Email).Data;
                if (userToCheck == null)
                {
                    return new ErrorDataResult<User>("Kullanıcı bulunamadı");
                }
            }

            if (!HashingHelper.VerifyPasswordHash(userForLoginDto.Password, userToCheck.PasswordHash, userToCheck.PasswordSalt))
            {
                return new ErrorDataResult<User>("Parola hatası");
            }

            return new SuccessDataResult<User>(userToCheck, "Başarılı giriş");
        }

        public IResult UserExists(string email, string userName)
        {
            if (_userService.GetByMail(email).Data != null)
            {
                return new ErrorResult("Email adresi sistemde kayıtlı");
            }
            else if (_userService.GetByUserName(userName).Data != null)
            {
                return new ErrorResult("Kullanıcı adı kullanılmakta");
            }
            return new SuccessResult();
        }

        public IDataResult<AccessToken> CreateAccessToken(IDataResult<User> dataResult)
        {
            var claims = _userService.GetClaims(dataResult.Data).Data;
            var accessToken = _tokenHelper.CreateToken(dataResult.Data, claims);
            return new SuccessDataResult<AccessToken>(accessToken, dataResult.Message);
        }
    }
}