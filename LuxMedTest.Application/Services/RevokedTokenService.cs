using LuxMedTest.Domain.Interfaces;

namespace LuxMedTest.Application.Services
{
    public class RevokedTokenService : IRevokedTokenService
    {
        private readonly HashSet<string> _revokedTokens = new();

        public void RevokeToken(string tokenId)
        {
            _revokedTokens.Add(tokenId);
        }

        public bool IsTokenRevoked(string tokenId)
        {
            return _revokedTokens.Contains(tokenId);
        }
    }
}
