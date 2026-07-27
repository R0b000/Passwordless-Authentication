using System.Security.Cryptography;
using System.Text;

namespace Auth.API.Utility.TokenHash
{
    /// <summary>
    /// Fast, deterministic hash for high-entropy tokens (refresh tokens, password
    /// reset tokens). BCrypt is intentionally avoided here because it is slow by
    /// design and non-deterministic (random salt), which makes it unsuitable for
    /// tokens that are only ever compared, never verified against a human-typed
    /// password.
    /// </summary>
    public static class TokenHasher
    {
        public static string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }
    }
}
