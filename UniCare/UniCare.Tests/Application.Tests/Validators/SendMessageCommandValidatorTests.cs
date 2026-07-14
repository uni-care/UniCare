using System;
using UniCare.Application.Chats.Commands.SendMessage;
using Xunit;

namespace UniCare.Tests.Application.Tests.Validators
{
    public class SendMessageCommandValidatorTests
    {
        private readonly SendMessageCommandValidator _validator = new();

        [Fact]
        public void ValidCommand_PassesValidation()
        {
            var command = new SendMessageCommand(Guid.NewGuid(), Guid.NewGuid(), "hello there");

            var result = _validator.Validate(command);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void EmptyBody_FailsValidation()
        {
            var command = new SendMessageCommand(Guid.NewGuid(), Guid.NewGuid(), "");

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "Body");
        }

        [Fact]
        public void BodyOver2000Characters_FailsValidation()
        {
            var command = new SendMessageCommand(Guid.NewGuid(), Guid.NewGuid(), new string('a', 2001));

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
        }

        [Fact]
        public void EmptyChatId_FailsValidation()
        {
            var command = new SendMessageCommand(Guid.Empty, Guid.NewGuid(), "hello");

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "ChatId");
        }

        [Fact]
        public void EmptySenderId_FailsValidation()
        {
            var command = new SendMessageCommand(Guid.NewGuid(), Guid.Empty, "hello");

            var result = _validator.Validate(command);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "SenderId");
        }
    }
}
