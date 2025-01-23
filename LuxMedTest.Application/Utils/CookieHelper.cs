using Microsoft.AspNetCore.Http;

namespace LuxMedTest.Application.Utils
{
    public static class CookieHelper
    {
        public static void SetTokenCookie(HttpResponse response, string cookieName, string token, DateTime expires)
        {
            response.Cookies.Append(cookieName, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expires
            });
        }
    }
}
