using EterationCaseStudy.Application.Abstractions;
using EterationCaseStudy.Domain.Repositories;
using MediatR;

namespace EterationCaseStudy.Application.Features.Users.Commands.SetUserRole
{
    public record SetUserRoleCommand(int Id, string Role) : IRequest<bool>;

    public class SetUserRoleHandler : IRequestHandler<SetUserRoleCommand, bool>
    {
        private readonly IUserRepository _users;
        private readonly IRoleRepository _roles;
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUser _currentUser;

        public SetUserRoleHandler(IUserRepository users, IRoleRepository roles, IUnitOfWork uow, ICurrentUser currentUser)
        {
            _users = users; _roles = roles; _uow = uow; _currentUser = currentUser;
        }

        public async Task<bool> Handle(SetUserRoleCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUser.IsAdmin) return false;

            var user = await _users.GetByIdAsync(request.Id, cancellationToken);
            if (user is null) return false;

            var role = (await _roles.ListAsync(r => r.Name == request.Role, cancellationToken)).FirstOrDefault();
            if (role is null) return false;

            user.SetRole(role);
            _users.Update(user);
            await _uow.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
