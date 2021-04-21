using Entities.DTOs;
using FluentValidation;

namespace Business.ValidationRules.FluentValidations
{
    public class UserForRegisterDtoValidator : AbstractValidator<UserForRegisterDto>
    {
        public UserForRegisterDtoValidator()
        {
            RuleFor(user => user.FirstName).NotEmpty();
            RuleFor(user => user.LastName).NotEmpty();
            RuleFor(user => user.Email).NotEmpty();
            RuleFor(user => user.Email).EmailAddress();
            RuleFor(u => u.UserName).NotEmpty();
            RuleFor(u => u.Address).NotEmpty();
            RuleFor(u => u.Password).NotEmpty();
            RuleFor(u => u.PhoneNumber).NotEmpty().Length(11);
            RuleFor(u => u.TCIdentityNumber).NotEmpty().Length(11);
        }
    }
}