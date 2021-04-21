using Entities.Concrete;
using FluentValidation;

namespace Business.ValidationRules.FluentValidations
{
    public class AddMoneyValidator : AbstractValidator<AddMoney>
    {
        public AddMoneyValidator()
        {
            RuleFor(w => w.UserId).NotEmpty();
            RuleFor(w => w.Amount).NotEmpty();
            RuleFor(w => w.Amount).GreaterThan(0);
        }
    }
}