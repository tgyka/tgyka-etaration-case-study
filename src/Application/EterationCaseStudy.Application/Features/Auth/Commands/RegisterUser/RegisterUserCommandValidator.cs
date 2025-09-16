using FluentValidation;
using EterationCaseStudy.Application.Features.Auth.Commands.RegisterUser;

namespace EterationCaseStudy.Application.Features.Auth.Commands
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
            RuleFor(x => x.Username).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Role).NotEmpty();
        }
    }
}
