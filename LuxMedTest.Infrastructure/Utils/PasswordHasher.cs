using LuxMedTest.Domain.Interfaces;
using System.Security.Cryptography;

namespace LuxMedTest.Infrastructure.Utils
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16; 
        private const int HashSize = 32;
        private const int Iterations = 100000;

        private static readonly HashAlgorithmName AlgorithmName = HashAlgorithmName.SHA256;

        public string HashPassword(string password)
        {
            var salt = GenerateSalt();
            var hash = HashPasswordWithSalt(password, salt);
            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split('.');
            if (parts.Length != 2)
            {
                throw new FormatException("Invalid hash format.");
            }

            var salt = Convert.FromBase64String(parts[0]);
            var hash = Convert.FromBase64String(parts[1]);

            var computedHash = HashPasswordWithSalt(password, salt);
            return CryptographicOperations.FixedTimeEquals(hash, computedHash);
        }

        private static byte[] GenerateSalt()
        {
            var salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private static byte[] HashPasswordWithSalt(string password, byte[] salt)
        {
            using var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations, AlgorithmName);
            return deriveBytes.GetBytes(HashSize);
        }
    }
}
