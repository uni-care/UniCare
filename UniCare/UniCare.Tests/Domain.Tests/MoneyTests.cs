using System;
using UniCare.Domain.VOs;
using Xunit;

namespace UniCare.Tests.Domain
{
    public class MoneyTests
    {
        [Fact]
        public void Create_WithNegativeAmount_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Money.Create(-1m, "USD"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("US")]
        [InlineData("USDD")]
        public void Create_WithInvalidCurrencyCode_ThrowsArgumentException(string currency)
        {
            Assert.Throws<ArgumentException>(() => Money.Create(10m, currency));
        }

        [Fact]
        public void Create_NormalizesCurrencyToUpperInvariant()
        {
            var money = Money.Create(10m, "usd");

            Assert.Equal("USD", money.Currency);
        }

        [Fact]
        public void Add_WithMatchingCurrency_SumsAmounts()
        {
            var a = Money.Create(10m, "USD");
            var b = Money.Create(5m, "USD");

            var result = a.Add(b);

            Assert.Equal(15m, result.Amount);
        }

        [Fact]
        public void Add_WithDifferentCurrencies_ThrowsInvalidOperationException()
        {
            var a = Money.Create(10m, "USD");
            var b = Money.Create(5m, "EGP");

            Assert.Throws<InvalidOperationException>(() => a.Add(b));
        }

        [Fact]
        public void Subtract_WithDifferentCurrencies_ThrowsInvalidOperationException()
        {
            var a = Money.Create(10m, "USD");
            var b = Money.Create(5m, "EGP");

            Assert.Throws<InvalidOperationException>(() => a.Subtract(b));
        }

        [Theory]
        [InlineData(0, true, false, false)]
        [InlineData(5, false, true, false)]
        [InlineData(-5, false, false, true)]
        public void StatePredicates_ReturnExpectedValues(
            decimal amount, bool expectedZero, bool expectedPositive, bool expectedNegative)
        {
            // Money.Create disallows negative amounts, so we exercise IsNegative via Subtract instead.
            if (amount < 0)
            {
                var money = Money.Create(0m, "USD").Subtract(Money.Create(-amount, "USD"));
                Assert.Equal(expectedNegative, money.IsNegative());
                return;
            }

            var m = Money.Create(amount, "USD");

            Assert.Equal(expectedZero, m.IsZero());
            Assert.Equal(expectedPositive, m.IsPositive());
            Assert.Equal(expectedNegative, m.IsNegative());
        }

        [Fact]
        public void Equality_SameAmountAndCurrency_AreEqual()
        {
            var a = Money.Create(10m, "USD");
            var b = Money.Create(10m, "USD");

            Assert.Equal(a, b);
            Assert.True(a == b);
        }
    }
}
