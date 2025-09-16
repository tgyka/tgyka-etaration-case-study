using EterationCaseStudy.Application.Features.Products.Commands;
using EterationCaseStudy.Application.Features.Products.Commands.CreateProduct;
using EterationCaseStudy.Application.Features.Products.Commands.DeleteProduct;
using EterationCaseStudy.Application.Features.Products.Commands.UpdateProduct;
using EterationCaseStudy.Domain.Entities;
using EterationCaseStudy.Domain.Repositories;
using FluentAssertions;
using Moq;

namespace EterationCaseStudy.UnitTest.Handlers;

public class ProductHandlersTests
{
    [Fact]
    public async Task CreateProduct_Should_Add_And_Return_Id()
    {
        var repo = new Mock<IProductRepository>();
        var uow = new Mock<IUnitOfWork>();
        var handler = new CreateProductHandler(repo.Object, uow.Object);

        repo.Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Callback<Product, CancellationToken>((p, _) => p.Id = 42)
            .Returns(Task.CompletedTask);
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var id = await handler.Handle(new CreateProductCommand("Phone", 1000m, 5, "desc"), CancellationToken.None);

        id.Should().Be(42);
        repo.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProduct_NotFound_Should_Return_False()
    {
        var repo = new Mock<IProductRepository>();
        var uow = new Mock<IUnitOfWork>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var handler = new UpdateProductHandler(repo.Object, uow.Object);
        var result = await handler.Handle(new UpdateProductCommand(1, "N", 10m, 1, null), CancellationToken.None);

        result.Should().BeFalse();
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteProduct_Found_Should_Remove_And_Return_True()
    {
        var repo = new Mock<IProductRepository>();
        var uow = new Mock<IUnitOfWork>();
        repo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Product("P", 1m, 1, "d") { Id = 1 });

        var handler = new DeleteProductHandler(repo.Object, uow.Object);
        var ok = await handler.Handle(new DeleteProductCommand(1), CancellationToken.None);

        ok.Should().BeTrue();
        repo.Verify(r => r.Remove(It.Is<Product>(p => p.Id == 1)), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateProduct_Found_Should_Update_And_Save()
    {
        var existing = new Product("Old", 5m, 2, "d") { Id = 10 };
        var repo = new Mock<IProductRepository>();
        var uow = new Mock<IUnitOfWork>();
        repo.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var handler = new UpdateProductHandler(repo.Object, uow.Object);
        var ok = await handler.Handle(new UpdateProductCommand(10, "NewName", 15m, 7, "new desc"), CancellationToken.None);

        ok.Should().BeTrue();
        existing.Name.Should().Be("NewName");
        existing.Price.Should().Be(15m);
        existing.StockQuantity.Should().Be(7);
        repo.Verify(r => r.Update(It.Is<Product>(p => p.Id == 10)), Times.Once);
        uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateProduct_Should_Pass_Correct_Properties_To_Repository()
    {
        var repo = new Mock<IProductRepository>();
        var uow = new Mock<IUnitOfWork>();
        Product? captured = null;

        repo.Setup(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Callback<Product, CancellationToken>((p, _) => { captured = p; p.Id = 1; })
            .Returns(Task.CompletedTask);
        uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var handler = new CreateProductHandler(repo.Object, uow.Object);
        await handler.Handle(new CreateProductCommand("Phone", 999.99m, 3, "Great phone"), CancellationToken.None);

        captured.Should().NotBeNull();
        captured!.Name.Should().Be("Phone");
        captured.Price.Should().Be(999.99m);
        captured.StockQuantity.Should().Be(3);
    }
}
