using System;
using UniCare.Domain.Aggregates.ChatAggregate;
using Xunit;

namespace UniCare.Tests.Domain
{
    public class ChatTests
    {
        [Fact]
        public void Create_WithSameOwnerAndRequester_ThrowsInvalidOperationException()
        {
            var userId = Guid.NewGuid();

            var act = () => Chat.Create(Guid.NewGuid(), userId, userId);

            Assert.Throws<InvalidOperationException>(act);
        }

        [Fact]
        public void EnsureParticipant_WithOwner_DoesNotThrow()
        {
            var ownerId = Guid.NewGuid();
            var chat = Chat.Create(Guid.NewGuid(), ownerId, Guid.NewGuid());

            var exception = Record.Exception(() => chat.EnsureParticipant(ownerId));

            Assert.Null(exception);
        }

        [Fact]
        public void EnsureParticipant_WithRequester_DoesNotThrow()
        {
            var requesterId = Guid.NewGuid();
            var chat = Chat.Create(Guid.NewGuid(), Guid.NewGuid(), requesterId);

            var exception = Record.Exception(() => chat.EnsureParticipant(requesterId));

            Assert.Null(exception);
        }

        [Fact]
        public void EnsureParticipant_WithStranger_ThrowsUnauthorizedAccessException()
        {
            var chat = Chat.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var act = () => chat.EnsureParticipant(Guid.NewGuid());

            Assert.Throws<UnauthorizedAccessException>(act);
        }
    }

    public class MessageTests
    {
        [Fact]
        public void Create_WithEmptyBody_ThrowsArgumentException()
        {
            var act = () => Message.Create(Guid.NewGuid(), Guid.NewGuid(), "   ");

            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void Create_WithBodyOver2000Characters_ThrowsArgumentException()
        {
            var longBody = new string('a', 2001);

            var act = () => Message.Create(Guid.NewGuid(), Guid.NewGuid(), longBody);

            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void Create_TrimsWhitespaceFromBody()
        {
            var message = Message.Create(Guid.NewGuid(), Guid.NewGuid(), "  hello  ");

            Assert.Equal("hello", message.Body);
        }

        [Fact]
        public void Create_DefaultsToTextMessageType()
        {
            var message = Message.Create(Guid.NewGuid(), Guid.NewGuid(), "hi");

            Assert.Equal(MessageType.Text, message.Type);
        }

        [Fact]
        public void MarkRead_FirstCall_SetsReadAt()
        {
            var message = Message.Create(Guid.NewGuid(), Guid.NewGuid(), "hi");

            message.MarkRead();

            Assert.NotNull(message.ReadAt);
        }

        [Fact]
        public void MarkRead_CalledTwice_IsIdempotent()
        {
            var message = Message.Create(Guid.NewGuid(), Guid.NewGuid(), "hi");

            message.MarkRead();
            var firstReadAt = message.ReadAt;
            message.MarkRead();

            Assert.Equal(firstReadAt, message.ReadAt);
        }
    }
}
