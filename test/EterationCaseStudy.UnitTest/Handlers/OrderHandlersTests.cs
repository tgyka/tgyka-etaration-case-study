using EterationCaseStudy.Application.Abstractions;
using EterationCaseStudy.Application.Features.Orders.Commands.CancelOrder;
using EterationCaseStudy.Application.Features.Orders.Commands.CreateOrder;
using EterationCaseStudy.Domain.Entities;
using EterationCaseStudy.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace EterationCaseStudy.UnitTest.Handlers;

public class OrderHandlersTests
{
    [Fact]
    public async Task CreateOrder_As_Admin_Should_Succeed()
    {
        var orders = new Mock<IOrderRepository>();
        var products = new Mock<IProductRepository>();
        var uow = new Mock<IUnitOfWork>();
        var currentUser = new Mock<ICurrentUser>();

        currentUser.SetupGet(c => c.IsAdmin).Returns(true);

        products.Setup(p => p.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product("P", 10m, 10, "d") { Id = 2 });

        orders.Setup(o => o.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Callback<Order, CancellationToken>((o, _) => o.Id = 101)
            .Returns(Task.CompletedTask);

        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateOrderHandler(orders.Object, products.Object, uow.Object, currentUser.Object);
        var id = await handler.Handle(new CreateOrderCommand(1, new List<CreateOrderItemDto> { new(2, 3) }), CancellationToken.None);

        id.Should().Be(101);
        orders.Verify(o => o.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CancelOrder_Different_User_NonAdmin_Should_Return_False()
    {
        var orders = new Mock<IOrderRepository>();
        var uow = new Mock<IUnitOfWork>();
        var currentUser = new Mock<ICurrentUser>();
        currentUser.SetupGet(c => c.IsAdmin).Returns(false);
        currentUser.SetupGet(c => c.UserId).Returns(99);

        var order = new Order(1) { Id = 10 };
        orders.Setup(o => o.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var handler = new CancelOrderHandler(orders.Object, uow.Object, currentUser.Object);
        var ok = await handler.Handle(new CancelOrderCommand(10), CancellationToken.None);

        ok.Should().BeFalse();
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateOrder_NonAdmin_DifferentUser_Should_Throw_Unauthorized()
    {
        var orders = new Mock<IOrderRepository>();
        var products = new Mock<IProductRepository>();
        var uow = new Mock<IUnitOfWork>();
        var currentUser = new Mock<ICurrentUser>();

        currentUser.SetupGet(c => c.IsAdmin).Returns(false);
        currentUser.SetupGet(c => c.UserId).Returns(5);

        var handler = new CreateOrderHandler(orders.Object, products.Object, uow.Object, currentUser.Object);
        var act = async () => await handler.Handle(new CreateOrderCommand(7, new List<CreateOrderItemDto> { new(2, 1) }), CancellationToken.None);
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task CreateOrder_InsufficientStock_Should_Throw()
    {
        var orders = new Mock<IOrderRepository>();
        var products = new Mock<IProductRepository>();
        var uow = new Mock<IUnitOfWork>();
        var currentUser = new Mock<ICurrentUser>();

        currentUser.SetupGet(c => c.IsAdmin).Returns(true);

        products.Setup(p => p.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product("P", 10m, 1, "d") { Id = 2 });

        var handler = new CreateOrderHandler(orders.Object, products.Object, uow.Object, currentUser.Object);
        var act = async () => await handler.Handle(new CreateOrderCommand(1, new List<CreateOrderItemDto> { new(2, 2) }), CancellationToken.None);
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Insufficient stock*");
    }
}
