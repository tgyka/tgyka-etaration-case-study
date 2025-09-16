using EterationCaseStudy.Application.Features.Orders.Commands;
using EterationCaseStudy.Application.Features.Orders.Commands.CancelOrder;
using EterationCaseStudy.Application.Features.Orders.Commands.CreateOrder;
using FluentAssertions;

namespace EterationCaseStudy.UnitTest.Validators;

public class OrderValidatorsTests
{
    [Fact]
    public void CreateOrder_Invalid_Should_Fail()
    {
        var v = new CreateOrderCommandValidator();
        var cmd = new CreateOrderCommand(0, new List<CreateOrderItemDto>());
        v.Validate(cmd).IsValid.Should().BeFalse();
    }

    [Fact]
    public void CreateOrder_Valid_Should_Pass()
    {
        var v = new CreateOrderCommandValidator();
        var cmd = new CreateOrderCommand(1, new List<CreateOrderItemDto> { new(2, 3) });
        v.Validate(cmd).IsValid.Should().BeTrue();
    }

    [Fact]
    public void CancelOrder_Invalid_Id_Should_Fail()
    {
        var v = new CancelOrderCommandValidator();
        v.Validate(new CancelOrderCommand(0)).IsValid.Should().BeFalse();
    }

    [Fact]
    public void CreateOrder_Item_With_Invalid_Fields_Should_Fail()
    {
        var v = new CreateOrderCommandValidator();
        var cmd = new CreateOrderCommand(1, new List<CreateOrderItemDto> { new(0, 0) });
        var result = v.Validate(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThan(0);
    }
}
