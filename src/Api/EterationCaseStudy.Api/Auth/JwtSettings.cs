namespace EterationCaseStudy.Api.Auth
{
    public class JwtSettings
    {
        public string Issuer { get; set; } = "EterationCaseStudy";
        public string Audience { get; set; } = "EterationClients";
        public string Key { get; set; } = "super_secret_key";
        public int ExpirationMinutes { get; set; } = 60;
    }
}

