using EterationCaseStudy.Domain.Entities;

namespace EterationCaseStudy.Application.Abstractions
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}