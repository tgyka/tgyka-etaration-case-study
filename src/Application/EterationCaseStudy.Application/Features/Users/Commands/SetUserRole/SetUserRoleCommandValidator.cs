using FluentValidation;

namespace EterationCaseStudy.Application.Features.Users.Commands.SetUserRole
{
    public class SetUserRoleCommandValidator : AbstractValidator<SetUserRoleCommand>
    {
        public SetUserRoleCommandValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Role).NotEmpty().MaximumLength(100);
        }
    }
}

