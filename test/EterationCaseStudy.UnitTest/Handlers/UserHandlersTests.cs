using EterationCaseStudy.Application.Abstractions;
using EterationCaseStudy.Application.Features.Users.Commands.SetUserRole;
using EterationCaseStudy.Application.Features.Users.Commands.UpdateUser;
using EterationCaseStudy.Domain.Entities;
using EterationCaseStudy.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace EterationCaseStudy.UnitTest.Handlers;

public class UserHandlersTests
{
    [Fact]
    public async Task UpdateUser_NonAdmin_DifferentUser_Should_Return_False()
    {
        var users = new Mock<IUserRepository>();
        var uow = new Mock<IUnitOfWork>();
        var current = new Mock<ICurrentUser>();

        var existing = new User("u","a@b.com","u") { Id = 1 };
        users.Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existing);

        current.SetupGet(c => c.IsAdmin).Returns(false);
        current.SetupGet(c => c.UserId).Returns(99);

        var handler = new UpdateUserHandler(users.Object, uow.Object, current.Object);
        var ok = await handler.Handle(new UpdateUserCommand(1, "newu", "a@b.com", "New U"), CancellationToken.None);
        ok.Should().BeFalse();
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SetUserRole_As_Admin_Should_Succeed()
    {
        var users = new Mock<IUserRepository>();
        var roles = new Mock<IRoleRepository>();
        var uow = new Mock<IUnitOfWork>();
        var current = new Mock<ICurrentUser>();

        current.SetupGet(c => c.IsAdmin).Returns(true);

        var user = new User("u","a@b.com","u") { Id = 10 };
        var role = new Role("Admin") { Id = 1 };
        users.Setup(u => u.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        roles.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Role> { role });

        var handler = new SetUserRoleHandler(users.Object, roles.Object, uow.Object, current.Object);
        var ok = await handler.Handle(new SetUserRoleCommand(10, "Admin"), CancellationToken.None);
        ok.Should().BeTrue();
        users.Verify(u => u.Update(It.Is<User>(x => x.Id == 10)), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_Self_NonAdmin_Should_Succeed()
    {
        var users = new Mock<IUserRepository>();
        var uow = new Mock<IUnitOfWork>();
        var current = new Mock<ICurrentUser>();

        var existing = new User("u","a@b.com","u") { Id = 7 };
        users.Setup(u => u.GetByIdAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(existing);

        current.SetupGet(c => c.IsAdmin).Returns(false);
        current.SetupGet(c => c.UserId).Returns(7);

        var handler = new UpdateUserHandler(users.Object, uow.Object, current.Object);
        var ok = await handler.Handle(new UpdateUserCommand(7, "newu", "new@b.com", "New U"), CancellationToken.None);
        ok.Should().BeTrue();
        users.Verify(u => u.Update(It.Is<User>(x => x.Id == 7)), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SetUserRole_Role_NotFound_Should_Return_False()
    {
        var users = new Mock<IUserRepository>();
        var roles = new Mock<IRoleRepository>();
        var uow = new Mock<IUnitOfWork>();
        var current = new Mock<ICurrentUser>();

        current.SetupGet(c => c.IsAdmin).Returns(true);

        var user = new User("u","a@b.com","u") { Id = 10 };
        users.Setup(u => u.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        roles.Setup(r => r.ListAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Role>());

        var handler = new SetUserRoleHandler(users.Object, roles.Object, uow.Object, current.Object);
        var ok = await handler.Handle(new SetUserRoleCommand(10, "Admin"), CancellationToken.None);
        ok.Should().BeFalse();
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
