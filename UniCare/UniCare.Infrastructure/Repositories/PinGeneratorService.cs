using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.TransactionHandoverAggregate;

namespace UniCare.Infrastructure.Repositories
{
    /// Generates cryptographically secure 6-digit PINs and hashes them with SHA-256.
    /// The raw PIN is NEVER persisted — only its hash is stored.
    public class PinGeneratorService : IPinGeneratorService
    {
        public string GeneratePin()
        {
            // RandomNumberGenerator is cryptographically secure (unlike System.Random)
            var randomBytes = new byte[4];
            RandomNumberGenerator.Fill(randomBytes);

            // Map to 0–999999 and zero-pad to 6 digits
            var value = (int)(BitConverter.ToUInt32(randomBytes, 0) % 1_000_000);
            return value.ToString("D6");
        }

        public string HashPin(string pin)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(pin));
            return Convert.ToHexString(bytes); // e.g. "A3F1..."
        }

        public bool VerifyPin(string rawPin, string storedHash)
        {
            var incomingHash = HashPin(rawPin);

            // CryptographicOperations.FixedTimeEquals prevents timing attacks
            var a = Encoding.UTF8.GetBytes(incomingHash);
            var b = Encoding.UTF8.GetBytes(storedHash);
            return CryptographicOperations.FixedTimeEquals(a, b);
        }
    }
}
