using EterationCaseStudy.Application.Abstractions;
using EterationCaseStudy.Domain.Repositories;
using MediatR;

namespace EterationCaseStudy.Application.Features.Users.Commands.UpdateUser
{
    public record UpdateUserCommand(int Id, string Username, string Email, string FullName) : IRequest<bool>;

    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IUserRepository _users;
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUser _currentUser;

        public UpdateUserHandler(IUserRepository users, IUnitOfWork uow, ICurrentUser currentUser)
        {
            _users = users; _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _users.GetByIdAsync(request.Id, cancellationToken);
            if (user is null) return false;

            if (!_currentUser.IsAdmin && _currentUser.UserId != request.Id)
                return false;

            user.UpdateProfile(request.Username, request.Email, request.FullName);
            _users.Update(user);
            await _uow.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
