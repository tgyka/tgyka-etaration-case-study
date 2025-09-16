using EterationCaseStudy.Application.Features.Products.Commands.DeleteProduct;
using FluentValidation;

namespace EterationCaseStudy.Application.Features.Products.Commands
{
    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
