using EterationCaseStudy.Application.Features.Auth.Commands;
using EterationCaseStudy.Application.Features.Auth.Commands.Login;
using EterationCaseStudy.Application.Features.Auth.Commands.RegisterUser;
using FluentAssertions;

namespace EterationCaseStudy.UnitTest.Validators;

public class AuthValidatorsTests
{
    [Fact]
    public void RegisterUser_Invalid_Should_Fail()
    {
        var v = new RegisterUserCommandValidator();
        var cmd = new RegisterUserCommand("bad", "123", "", "");
        v.Validate(cmd).IsValid.Should().BeFalse();
    }

    [Fact]
    public void RegisterUser_Valid_Should_Pass()
    {
        var v = new RegisterUserCommandValidator();
        var cmd = new RegisterUserCommand("a@b.com", "Secret1", "user", "User");
        v.Validate(cmd).IsValid.Should().BeTrue();
    }

    [Fact]
    public void Login_Invalid_Should_Fail()
    {
        var v = new LoginCommandValidator();
        var cmd = new LoginCommand("not-an-email", "");
        v.Validate(cmd).IsValid.Should().BeFalse();
    }

    [Fact]
    public void RegisterUser_ShortPassword_Should_Fail()
    {
        var v = new RegisterUserCommandValidator();
        var cmd = new RegisterUserCommand("a@b.com", "123", "user", "User");
        v.Validate(cmd).IsValid.Should().BeFalse();
    }
}
