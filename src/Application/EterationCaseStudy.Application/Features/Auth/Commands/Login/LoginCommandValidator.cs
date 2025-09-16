using FluentValidation;
using EterationCaseStudy.Application.Features.Auth.Commands.Login;

namespace EterationCaseStudy.Application.Features.Auth.Commands
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
