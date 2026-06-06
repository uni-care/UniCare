using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.VOs;

namespace UniCare.Infrastructure.Persistence.Converters
{
    public class MoneyToStringConverter : ValueConverter<Money, string>
    {
        public MoneyToStringConverter()
            : base(
                // Convert Money → string for saving
                money => money != null ? $"{money.Amount}:{money.Currency}" : null,

                // Convert string → Money for reading
                dbValue => ParseMoney(dbValue)
            )
        {
        }

        private static Money ParseMoney(string dbValue)
        {
            if (string.IsNullOrWhiteSpace(dbValue))
                return Money.Create(0, "USD"); // or throw, depending on your nullability

            var parts = dbValue.Split(':');
            if (parts.Length == 2 && decimal.TryParse(parts[0], out var amount))
            {
                return Money.Create(amount, parts[1]);
            }

            // Fallback for invalid data
            return Money.Create(0, "USD");
        }
    }
}
