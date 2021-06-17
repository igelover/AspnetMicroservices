using FluentValidation;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutOrderCommandValidator : AbstractValidator<CheckoutOrderCommand>
    {
        public CheckoutOrderCommandValidator()
        {
            RuleFor(cmd => cmd.Order).NotNull().WithMessage("{Order} is required.");
            RuleFor(cmd => cmd.Order.UserName)
                .NotEmpty().WithMessage("{UserName} is required.")
                .NotNull().MaximumLength(50).WithMessage("{UserName} must not exceed 50 characters.");
            RuleFor(cmd => cmd.Order.EmailAddress)
                .NotEmpty().WithMessage("{EmailAddress} is required.");
            RuleFor(cmd => cmd.Order.TotalPrice)
                .NotEmpty().WithMessage("{TotalPrice} is required.")
                .GreaterThan(0).WithMessage("{TotalPrice} should be greater than 0.");
        }
    }
}
