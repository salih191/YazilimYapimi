using Entities.Concrete;
using FluentValidation;

namespace Business.ValidationRules.FluentValidations
{
    public class AddProductValidator : AbstractValidator<AddProduct>
    {
        public AddProductValidator()
        {
            RuleFor(p => p.CategoryId).NotEmpty();
            RuleFor(p => p.Quantity).NotEmpty();
            RuleFor(p => p.Quantity).GreaterThan(0);
            RuleFor(p => p.UnitPrice).NotEmpty();
            RuleFor(p => p.UnitPrice).GreaterThan(0);
            RuleFor(p => p.SupplierId).NotEmpty();
        }
    }
}