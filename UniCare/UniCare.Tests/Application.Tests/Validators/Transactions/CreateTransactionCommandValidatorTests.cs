using System;
using UniCare.Application.Transactions.Commands.CreateTransaction;
using UniCare.Domain.Aggregates.TransactionAggregate;
using Xunit;

namespace UniCare.Tests.Application.Tests.Validators.Transactions
{
    public class CreateTransactionCommandValidatorTests
    {
        private readonly CreateTransactionCommandValidator _validator = new();

        [Fact]
        public void ValidSaleCommand_PassesValidation()
        {
            var command = new CreateTransactionCommand(
                ItemId: Guid.NewGuid(),
                OwnerId: Guid.NewGuid(),
                RequesterId: Guid.NewGuid(),
                Type: TransactionType.Sale,
                AgreedPrice: 50m,
                RentalReturnDue: null);

            var result = _validator.Validate(command);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void ValidRentalCommand_PassesValidation()
        {
            var command = new CreateTransactionCommand(
                ItemId: Guid.NewGuid(),
                OwnerId: Guid.NewGuid(),
                RequesterId: Guid.NewGuid(),
                Type: TransactionType.Rental,
                AgreedPrice: 50m,
                RentalReturnDue: DateTime.UtcNow.AddDays(5));

            var result = _validator.Validate(command);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void RentalWithoutReturnDue_FailsValidation()
        {
            var command = new CreateTransactionCommand(
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                TransactionType.Rental, 50m, RentalReturnDue: null);

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "RentalReturnDue");
        }

        [Fact]
        public void RentalWithPastReturnDue_FailsValidation()
        {
            var command = new CreateTransactionCommand(
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                TransactionType.Rental, 50m, RentalReturnDue: DateTime.UtcNow.AddDays(-1));

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void SameOwnerAndRequester_FailsValidation()
        {
            var userId = Guid.NewGuid();
            var command = new CreateTransactionCommand(
                Guid.NewGuid(), userId, userId,
                TransactionType.Sale, 50m, null);

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void ZeroAgreedPrice_FailsValidation()
        {
            var command = new CreateTransactionCommand(
                Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
                TransactionType.Sale, 0m, null);

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "AgreedPrice");
        }

        [Fact]
        public void EmptyItemId_FailsValidation()
        {
            var command = new CreateTransactionCommand(
                Guid.Empty, Guid.NewGuid(), Guid.NewGuid(),
                TransactionType.Sale, 50m, null);

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "ItemId");
        }
    }
}
