namespace EterationCaseStudy.Application.Abstractions
{
    public interface ICurrentUser
    {
        int? UserId { get; }
        string? Username { get; }
        bool IsAuthenticated { get; }
        bool IsAdmin { get; }
    }
}

