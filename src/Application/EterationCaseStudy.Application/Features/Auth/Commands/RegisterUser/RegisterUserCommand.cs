using EterationCaseStudy.Application.Common.Security;
using EterationCaseStudy.Application.Features.Auth.Dto;
using EterationCaseStudy.Domain.Entities;
using EterationCaseStudy.Domain.Repositories;
using MediatR;

namespace EterationCaseStudy.Application.Features.Auth.Commands.RegisterUser
{
    public record RegisterUserCommand(
        string Email,
        string Password,
        string Username,
        string Role
    ) : IRequest<RegisterResultDto>;

    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, RegisterResultDto>
    {
        private readonly IUserRepository _users;
        private readonly IRoleRepository _roles;
        private readonly IUnitOfWork _uow;

        public RegisterUserHandler(IUserRepository users, IRoleRepository roles, IUnitOfWork uow)
        {
            _users = users;
            _roles = roles;
            _uow = uow;
        }

        public async Task<RegisterResultDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var existing = (await _users.ListAsync(u => u.Email == request.Email, cancellationToken)).FirstOrDefault();
            if (existing != null)
                throw new InvalidOperationException("Email already registered");

            var username = string.IsNullOrWhiteSpace(request.Username) ? request.Email : request.Username;
            var fullName = username;
            var user = new User(username, request.Email, fullName);
            user.SetPasswordHash(PasswordHasher.Hash(request.Password));
            var roleName = string.IsNullOrWhiteSpace(request.Role) ? "User" : request.Role.Trim();
            var role = (await _roles.ListAsync(r => r.Name == roleName, cancellationToken)).FirstOrDefault()
                       ?? (await _roles.ListAsync(r => r.Name == "User", cancellationToken)).First();
            user.SetRole(role);

            await _users.AddAsync(user, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return new RegisterResultDto(user.Id, user.Email);
        }
    }
}
