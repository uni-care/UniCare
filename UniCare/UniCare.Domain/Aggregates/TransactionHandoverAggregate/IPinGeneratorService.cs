using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Domain.Aggregates.TransactionHandoverAggregate
{
    public interface IPinGeneratorService
    {
        // Generates a cryptographically secure 6-digit PIN.
        string GeneratePin();

        // Returns the SHA-256 hash of the given PIN for safe storage.
        string HashPin(string pin);

        // Verifies a raw PIN against a stored hash.
        bool VerifyPin(string rawPin, string storedHash);
    }
}
