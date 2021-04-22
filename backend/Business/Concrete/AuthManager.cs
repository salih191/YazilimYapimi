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
            HashingHelper.CreatePasswordHash(password, out passwordHash, out passwordSalt);//şifre hashlaniyor
            var user = new User//biligiler ile yeni kullanıcı oluşuyor
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
            _userService.Add(user);//user tablosuna ekleniyor
            return new SuccessDataResult<User>(user, "Kayıt oldu");
        }

        public IDataResult<User> Login(UserForLoginDto userForLoginDto)
        {
            var userToCheck = _userService.GetByMail(userForLoginDto.Email).Data;//email ile kullanıcı bilgileri çekiliyor
            if (userToCheck == null)//veri yoksa username üzerinden deneniyor yine veri yoksa kullanıcı bulunamadı mesajı
            {
                userToCheck = _userService.GetByUserName(userForLoginDto.Email).Data;
                if (userToCheck == null)
                {
                    return new ErrorDataResult<User>("Kullanıcı bulunamadı");
                }
            }

            if (!HashingHelper.VerifyPasswordHash(userForLoginDto.Password, userToCheck.PasswordHash, userToCheck.PasswordSalt))//şifre doğru mu
            {
                return new ErrorDataResult<User>("Parola hatası");
            }

            return new SuccessDataResult<User>(userToCheck, "Başarılı giriş");
        }

        public IResult UserExists(string email, string userName)//email ve kullanıcı adı eşsiz mi
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

        public IDataResult<AccessToken> CreateAccessToken(IDataResult<User> dataResult)//jwt oluşturma
        {
            var claims = _userService.GetClaims(dataResult.Data).Data;//user üzerinden rolleri çekiliyor
            var accessToken = _tokenHelper.CreateToken(dataResult.Data, claims);//jwt oluşuyor
            return new SuccessDataResult<AccessToken>(accessToken, dataResult.Message);
        }
    }
}