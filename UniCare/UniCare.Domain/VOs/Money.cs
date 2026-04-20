using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Common;

namespace UniCare.Domain.VOs
{
    public class Money : ValueObject
    {
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }

        private Money() { } // EF Core

        private Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public static Money Create(decimal amount, string currency)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative.", nameof(amount));

            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency is required.", nameof(currency));

            if (currency.Length != 3)
                throw new ArgumentException("Currency must be a 3-letter ISO code (e.g., USD, EUR, EGP).", nameof(currency));

            return new Money(amount, currency.ToUpperInvariant());
        }

        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException($"Cannot add money with different currencies: {Currency} and {other.Currency}");

            return new Money(Amount + other.Amount, Currency);
        }

        public Money Subtract(Money other)
        {
            if (Currency != other.Currency)
                throw new InvalidOperationException($"Cannot subtract money with different currencies: {Currency} and {other.Currency}");

            return new Money(Amount - other.Amount, Currency);
        }

        public Money Multiply(decimal multiplier)
        {
            return new Money(Amount * multiplier, Currency);
        }

        public bool IsZero() => Amount == 0;

        public bool IsPositive() => Amount > 0;

        public bool IsNegative() => Amount < 0;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        public override string ToString() => $"{Amount:N2} {Currency}";
    }
}
