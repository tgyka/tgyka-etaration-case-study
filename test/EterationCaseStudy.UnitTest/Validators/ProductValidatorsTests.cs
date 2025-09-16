using EterationCaseStudy.Application.Features.Products.Commands;
using EterationCaseStudy.Application.Features.Products.Commands.CreateProduct;
using EterationCaseStudy.Application.Features.Products.Commands.DeleteProduct;
using EterationCaseStudy.Application.Features.Products.Commands.UpdateProduct;
using FluentAssertions;

namespace EterationCaseStudy.UnitTest.Validators;

public class ProductValidatorsTests
{
    [Fact]
    public void CreateProduct_Valid_Should_Pass()
    {
        var v = new CreateProductCommandValidator();
        var cmd = new CreateProductCommand("Phone", 1000m, 10, "desc");
        var result = v.Validate(cmd);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void CreateProduct_Invalid_Should_Fail()
    {
        var v = new CreateProductCommandValidator();
        var cmd = new CreateProductCommand("", -5m, -1, null);
        var result = v.Validate(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void UpdateProduct_Invalid_Id_Should_Fail()
    {
        var v = new UpdateProductCommandValidator();
        var cmd = new UpdateProductCommand(0, "Name", 10m, 1, null);
        v.Validate(cmd).IsValid.Should().BeFalse();
    }

    [Fact]
    public void DeleteProduct_Invalid_Id_Should_Fail()
    {
        var v = new DeleteProductCommandValidator();
        var cmd = new DeleteProductCommand(0);
        v.Validate(cmd).IsValid.Should().BeFalse();
    }
}

