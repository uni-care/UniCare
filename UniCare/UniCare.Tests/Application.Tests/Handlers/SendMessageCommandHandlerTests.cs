using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using UniCare.Application.Chats.Commands.SendMessage;
using UniCare.Domain.Aggregates.ChatAggregate;
using Xunit;

namespace UniCare.Tests.Application.Tests.Handlers
{
    public class SendMessageCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ChatNotFound_ReturnsFailure()
        {
            var chatRepoMock = new Mock<IChatRepository>();
            chatRepoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Chat?)null);

            var handler = new SendMessageCommandHandler(
                chatRepoMock.Object, Mock.Of<IChatNotificationService>());

            var command = new SendMessageCommand(Guid.NewGuid(), Guid.NewGuid(), "hello");

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_SenderNotAParticipant_ReturnsFailure()
        {
            var chat = Chat.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var chatRepoMock = new Mock<IChatRepository>();
            chatRepoMock
                .Setup(r => r.GetByIdAsync(chat.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(chat);

            var handler = new SendMessageCommandHandler(
                chatRepoMock.Object, Mock.Of<IChatNotificationService>());

            // Random sender who is neither the owner nor the requester of the chat.
            var command = new SendMessageCommand(chat.Id, Guid.NewGuid(), "hello");

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ValidMessage_PersistsAndNotifies()
        {
            var ownerId = Guid.NewGuid();
            var requesterId = Guid.NewGuid();
            var chat = Chat.Create(Guid.NewGuid(), ownerId, requesterId);

            var chatRepoMock = new Mock<IChatRepository>();
            chatRepoMock
                .Setup(r => r.GetByIdAsync(chat.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(chat);

            var notificationMock = new Mock<IChatNotificationService>();

            var handler = new SendMessageCommandHandler(chatRepoMock.Object, notificationMock.Object);

            var command = new SendMessageCommand(chat.Id, ownerId, "Hello there");

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal("Hello there", result.Data!.Body);
            Assert.Equal(ownerId, result.Data.SenderId);

            chatRepoMock.Verify(r => r.AddMessageAsync(
                It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Once);

            notificationMock.Verify(n => n.NotifyMessageSentAsync(
                chat.Id, It.IsAny<MessageDto>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_EmptyBody_ReturnsFailureAndDoesNotNotify()
        {
            // Defensive check on the domain rule (Message.Create) — normally the
            // ValidationBehavior pipeline would reject this before it reaches the handler.
            var ownerId = Guid.NewGuid();
            var chat = Chat.Create(Guid.NewGuid(), ownerId, Guid.NewGuid());

            var chatRepoMock = new Mock<IChatRepository>();
            chatRepoMock
                .Setup(r => r.GetByIdAsync(chat.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync(chat);

            var notificationMock = new Mock<IChatNotificationService>();
            var handler = new SendMessageCommandHandler(chatRepoMock.Object, notificationMock.Object);

            var command = new SendMessageCommand(chat.Id, ownerId, "   ");

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result.IsSuccess);
            notificationMock.Verify(n => n.NotifyMessageSentAsync(
                It.IsAny<Guid>(), It.IsAny<MessageDto>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
