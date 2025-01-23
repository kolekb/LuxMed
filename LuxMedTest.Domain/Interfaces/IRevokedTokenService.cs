namespace LuxMedTest.Domain.Interfaces
{
    public interface IRevokedTokenService
    {
        void RevokeToken(string tokenId);
        bool IsTokenRevoked(string tokenId);
    }
}
