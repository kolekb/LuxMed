namespace LuxMedTest.Application.Settings
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int TokenExpiryMinutes { get; set; } = 60;
        public int SlidingExpirationThresholdMinutes { get; set; } = 10;
        public string CookieName { get; set; } = "AuthToken";

    }
}
