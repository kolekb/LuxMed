using LuxMedTest.Application.Settings;
using LuxMedTest.Domain.Interfaces;

namespace LuxMedTest.Api.Middlewares
{
    public class CookieAuthenticationMiddleware(RequestDelegate next,
            IJwtTokenService jwtTokenService,
            JwtSettings settings,
            ILogger<CookieAuthenticationMiddleware> logger)
    {
        private const string LogoutPath = "/api/auth/logout";
        private const string LoginPath = "/api/auth/login";

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;

            // Skip authentication for Swagger, login, and logout
            if (path != null && (path.StartsWith("/swagger") 
                || path.StartsWith(LoginPath) 
                || path.StartsWith(LogoutPath)))
            {
                await next(context);
                return;
            }

            if (context.Request.Cookies.TryGetValue(settings.CookieName, out var token))
            {
                var principal = jwtTokenService.ValidateToken(token);

                if (principal != null)
                {
                    logger.LogInformation("Valid token found in cookie.");
                    context.User = principal;
                    await next(context);
                    return;
                }

                context.Response.Cookies.Delete(settings.CookieName);
                return;
            }
            var message = "No authentication token found in cookies.";
            logger.LogWarning(message);
            throw new UnauthorizedAccessException(message);
        }
    }
}
