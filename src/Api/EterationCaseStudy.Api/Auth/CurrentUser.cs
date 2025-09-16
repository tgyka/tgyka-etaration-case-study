using System.Security.Claims;
using EterationCaseStudy.Application.Abstractions;

namespace EterationCaseStudy.Api.Auth
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

        public int? UserId
        {
            get
            {
                var idStr = Principal?.FindFirstValue(ClaimTypes.NameIdentifier) ?? Principal?.FindFirstValue("sub");
                return int.TryParse(idStr, out var id) ? id : null;
            }
        }

        public string? Username => Principal?.Identity?.Name ?? Principal?.FindFirstValue(ClaimTypes.Name);

        public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

        public bool IsAdmin => Principal?.IsInRole("Admin") == true;
    }
}

