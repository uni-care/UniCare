using System;
using UniCare.Application.Item.Commands.CreateItem;
using UniCare.Domain.Enums;
using Xunit;

namespace UniCare.Tests.Application.Tests.Validators.Items
{
    public class CreateItemCommandValidatorTests
    {
        private readonly CreateItemCommandValidator _validator = new();

        private static CreateItemCommand ValidCommand() => new(
            "Bike for sale",
            "A slightly used mountain bike",
            100m,
            ItemType.ForSale,
            "USD",
            Guid.NewGuid(),
            null,
            null,
            "Cairo",
            null,
            Guid.NewGuid()
        );

        [Fact]
        public void ValidCommand_PassesValidation()
        {
            var result = _validator.Validate(ValidCommand());
            Assert.True(result.IsValid);
        }

        [Fact]
        public void EmptyTitle_FailsValidation()
        {
            var command = ValidCommand() with { Title = "" };
            var result = _validator.Validate(command);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Title");
        }

        [Fact]
        public void TitleOver200Characters_FailsValidation()
        {
            var command = ValidCommand() with { Title = new string('a', 201) };
            var result = _validator.Validate(command);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Title");
        }

        [Fact]
        public void EmptyDescription_FailsValidation()
        {
            var command = ValidCommand() with { Description = "" };
            var result = _validator.Validate(command);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Description");
        }

        [Fact]
        public void DescriptionOver2000Characters_FailsValidation()
        {
            var command = ValidCommand() with { Description = new string('a', 2001) };
            var result = _validator.Validate(command);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Description");
        }

        [Fact]
        public void ZeroPrice_FailsValidation()
        {
            var command = ValidCommand() with { Price = 0 };
            var result = _validator.Validate(command);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Price");
        }

        [Fact]
        public void NegativePrice_FailsValidation()
        {
            var command = ValidCommand() with { Price = -10 };
            var result = _validator.Validate(command);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Price");
        }

        [Fact]
        public void InvalidItemType_FailsValidation()
        {
            var command = ValidCommand() with { ItemType = (ItemType)999 };
            var result = _validator.Validate(command);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "ItemType");
        }

        [Fact]
        public void EmptyCurrency_FailsValidation()
        {
            var command = ValidCommand() with { Currency = "" };
            var result = _validator.Validate(command);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Currency");
        }

        [Fact]
        public void CurrencyNotThreeCharacters_FailsValidation()
        {
            var command = ValidCommand() with { Currency = "US" };
            var result = _validator.Validate(command);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Currency");
        }

        [Fact]
        public void EmptyOwnerId_FailsValidation()
        {
            var command = ValidCommand() with { OwnerId = Guid.Empty };
            var result = _validator.Validate(command);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "OwnerId");
        }

        [Fact]
        public void EmptyCategoryId_FailsValidation()
        {
            var command = ValidCommand() with { CategoryId = Guid.Empty };
            var result = _validator.Validate(command);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "CategoryId");
        }

        [Fact]
        public void AvailableToBeforeAvailableFrom_FailsValidation()
        {
            var command = ValidCommand() with
            {
                AvailableFrom = DateTime.UtcNow,
                AvailableTo = DateTime.UtcNow.AddDays(-1)
            };
            var result = _validator.Validate(command);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "AvailableTo");
        }
    }
}