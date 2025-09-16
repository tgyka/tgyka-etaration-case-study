using EterationCaseStudy.Application.Features.Users.Commands.SetUserRole;
using EterationCaseStudy.Application.Features.Users.Commands.UpdateUser;
using FluentAssertions;

namespace EterationCaseStudy.UnitTest.Validators;

public class UserValidatorsTests
{
    [Fact]
    public void UpdateUser_Invalid_Should_Fail()
    {
        var v = new UpdateUserCommandValidator();
        var cmd = new UpdateUserCommand(0, "", "bad", "");
        v.Validate(cmd).IsValid.Should().BeFalse();
    }

    [Fact]
    public void SetUserRole_Invalid_Should_Fail()
    {
        var v = new SetUserRoleCommandValidator();
        var cmd = new SetUserRoleCommand(0, "");
        v.Validate(cmd).IsValid.Should().BeFalse();
    }
}

