using EterationCaseStudy.Application.Abstractions;
using EterationCaseStudy.Application.Common.Security;
using EterationCaseStudy.Application.Features.Auth.Commands.Login;
using EterationCaseStudy.Application.Features.Auth.Commands.RegisterUser;
using EterationCaseStudy.Domain.Entities;
using EterationCaseStudy.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace EterationCaseStudy.UnitTest.Handlers;

public class AuthHandlersTests
{
    [Fact]
    public async Task RegisterUser_Duplicate_Email_Should_Throw()
    {
        var users = new Mock<IUserRepository>();
        var roles = new Mock<IRoleRepository>();
        var uow = new Mock<IUnitOfWork>();

        users.Setup(u => u.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { new("u","a@b.com","u") { Id = 1 } });

        var handler = new RegisterUserHandler(users.Object, roles.Object, uow.Object);
        var act = async () => await handler.Handle(new RegisterUserCommand("a@b.com", "Secret1", "u", "User"), CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Login_Invalid_Password_Should_Throw_Unauthorized()
    {
        var users = new Mock<IUserRepository>();
        var roles = new Mock<IRoleRepository>();
        var jwt = new Mock<IJwtTokenGenerator>();

        var user = new User("u","a@b.com","u") { Id = 5 };
        user.SetPasswordHash(PasswordHasher.Hash("Correct1"));

        users.Setup(u => u.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { user });

        roles.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Role("User") { Id = 2 });

        var handler = new LoginHandler(users.Object, roles.Object, jwt.Object);
        var act = async () => await handler.Handle(new LoginCommand("a@b.com", "Wrong"), CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task Login_UserNotFound_Should_Throw_Unauthorized()
    {
        var users = new Mock<IUserRepository>();
        var roles = new Mock<IRoleRepository>();
        var jwt = new Mock<IJwtTokenGenerator>();

        users.Setup(u => u.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        var handler = new LoginHandler(users.Object, roles.Object, jwt.Object);
        var act = async () => await handler.Handle(new LoginCommand("a@b.com", "Secret1"), CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task RegisterUser_NoRoleProvided_Should_Assign_Default_User_Role()
    {
        var users = new Mock<IUserRepository>();
        var roles = new Mock<IRoleRepository>();
        var uow = new Mock<IUnitOfWork>();

        users.Setup(u => u.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        var defaultRole = new Role("User") { Id = 2 };
        roles.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Role> { defaultRole });

        User? captured = null;
        users.Setup(u => u.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, _) => captured = user)
            .Returns(Task.CompletedTask);

        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new RegisterUserHandler(users.Object, roles.Object, uow.Object);
        await handler.Handle(new RegisterUserCommand("a@b.com", "Secret1", "user", ""), CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.RoleId.Should().Be(2);
    }
}
