using EterationCaseStudy.Application.Features.Orders.Commands.CreateOrder;
using FluentValidation;

namespace EterationCaseStudy.Application.Features.Orders.Commands
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.UserId).GreaterThan(0);
            RuleFor(x => x.Items).NotNull().NotEmpty();
            RuleForEach(x => x.Items).ChildRules(items =>
            {
                items.RuleFor(i => i.ProductId).GreaterThan(0);
                items.RuleFor(i => i.Quantity).GreaterThan(0);
            });
        }
    }
}
