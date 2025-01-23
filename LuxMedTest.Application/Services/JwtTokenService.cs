using LuxMedTest.Application.Settings;
using LuxMedTest.Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LuxMedTest.Application.Services
{
    public class JwtTokenService(IRevokedTokenService revokedTokenService, JwtSettings jwtSettings) : IJwtTokenService
    {
        public string WriteToken(string username, DateTime expires)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = jwtSettings.SecretKey;
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, username)
                ]),
                Expires = DateTime.UtcNow.AddMinutes(jwtSettings.TokenExpiryMinutes),
                Issuer = jwtSettings.Issuer,
                Audience = jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Claims = new Dictionary<string, object> { { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() } }
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out var securityToken);

                // Sprawdź, czy token jest unieważniony
                var jwtToken = securityToken as JwtSecurityToken;
                if (jwtToken == null || revokedTokenService.IsTokenRevoked(jwtToken.Id))
                {
                    return null;
                }

                return principal;
            }
            catch (SecurityTokenExpiredException)
            {
                // Token jest przeterminowany
                return null;
            }
            catch (SecurityTokenException)
            {
                // Token jest nieprawidłowy
                return null;
            }
        }

        public bool ShouldRenewToken(JwtSecurityToken jwtToken)
        {
            var timeToExpiration = jwtToken.ValidTo - DateTime.UtcNow;
            return timeToExpiration.TotalMinutes <= jwtSettings.SlidingExpirationThresholdMinutes;
        }

        public string RenewToken(ClaimsPrincipal principal, DateTime expires)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

            var claims = principal.Claims.ToList();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Claims = new Dictionary<string, object> { { JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() } }
            };

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }

        public static string GenerateSecret()
        {
            var key = new byte[32]; // 256 bitów
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(key);
            }
            var secretKey = Convert.ToBase64String(key);
            Console.WriteLine($"Generated Key: {secretKey}");
            return secretKey;
        }
    }
}
