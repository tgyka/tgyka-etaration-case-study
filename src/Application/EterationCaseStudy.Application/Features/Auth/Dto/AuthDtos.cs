namespace EterationCaseStudy.Application.Features.Auth.Dto
{
    public record RegisterResultDto(int Id, string Email);
    public record LoginResultDto(string Token);
}