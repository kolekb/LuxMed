using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LuxMedTest.Domain.Interfaces
{
    public interface IJwtTokenService
    {
        ClaimsPrincipal? ValidateToken(string token);
        string WriteToken(string username, DateTime expires);
        bool ShouldRenewToken(JwtSecurityToken jwtToken);
        string RenewToken(ClaimsPrincipal principal, DateTime expires);
    }
}
