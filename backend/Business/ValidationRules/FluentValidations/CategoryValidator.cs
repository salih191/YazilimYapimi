using Entities.Concrete;
using FluentValidation;

namespace Business.ValidationRules.FluentValidations
{
    public class CategoryValidator : AbstractValidator<Category>
    {
        public CategoryValidator()
        {
            RuleFor(c => c.Name).NotEmpty();
        }
    }
}