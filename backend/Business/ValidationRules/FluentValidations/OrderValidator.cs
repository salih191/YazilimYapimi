using Entities.Concrete;
using FluentValidation;

namespace Business.ValidationRules.FluentValidations
{
    public class OrderValidator:AbstractValidator<Order>
    {
        public OrderValidator()
        {
            RuleFor(o => o.CategoryId).NotEmpty();
            RuleFor(o => o.Quantity).NotEmpty();
            RuleFor(o => o.CustomerId).NotEmpty();
        }
    }
}