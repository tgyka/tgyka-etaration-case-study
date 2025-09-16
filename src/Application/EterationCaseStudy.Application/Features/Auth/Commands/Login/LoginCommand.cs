using EterationCaseStudy.Application.Abstractions;
using EterationCaseStudy.Application.Common.Security;
using EterationCaseStudy.Application.Features.Auth.Dto;
using EterationCaseStudy.Domain.Repositories;
using MediatR;

namespace EterationCaseStudy.Application.Features.Auth.Commands.Login
{
    public record LoginCommand(string Email, string Password) : IRequest<LoginResultDto>;

    public class LoginHandler : IRequestHandler<LoginCommand, LoginResultDto>
    {
        private readonly IUserRepository _users;
        private readonly IRoleRepository _roles;
        private readonly IJwtTokenGenerator _jwt;

        public LoginHandler(IUserRepository users, IRoleRepository roles, IJwtTokenGenerator jwt)
        {
            _users = users;
            _roles = roles;
            _jwt = jwt;
        }

        public async Task<LoginResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = (await _users.ListAsync(u => u.Email == request.Email, cancellationToken)).FirstOrDefault();
            if (user == null) throw new UnauthorizedAccessException("Invalid credentials");
            if (string.IsNullOrEmpty(user.PasswordHash) || !PasswordHasher.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            var role = await _roles.GetByIdAsync(user.RoleId, cancellationToken) ??
                       (await _roles.ListAsync(r => r.Name == "User", cancellationToken)).First();
            user.SetRole(role);

            var token = _jwt.GenerateToken(user);
            return new LoginResultDto(token);
        }
    }
}
