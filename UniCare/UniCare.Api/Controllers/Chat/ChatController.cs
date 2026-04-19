using MediatR;
using Microsoft.AspNetCore.Mvc;
using UniCare.Api.Controllers.Chat.Requests;
using UniCare.Application.Chats.Commands.GetOrCreateChat;
using UniCare.Application.Chats.Commands.MarkMessagesRead;
using UniCare.Application.Chats.Commands.SendMessage;
using UniCare.Application.Chats.Queries.GetConversation;
using UniCare.Application.Chats.Queries.GetUserChats;
using UniCare.Domain.Aggregates.ChatAggregate;

namespace UniCare.Api.Controllers.Chat
{
    [ApiController]
    [Route("api/chats")]
    public class ChatController : ControllerBase
    {
        private readonly ISender _sender;

        public ChatController(ISender sender) => _sender = sender;

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<ChatSummaryResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserChats(
            [FromQuery] Guid userId,
            CancellationToken ct)
        {
            var result = await _sender.Send(new GetUserChatsQuery(userId), ct);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
        }

        [HttpPost("for-transaction")]
        [ProducesResponseType(typeof(GetOrCreateChatResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOrCreate(
            [FromBody] GetOrCreateChatRequest request,
            CancellationToken ct)
        {
            var command = new GetOrCreateChatCommand(
                TransactionId: request.TransactionId,
                OwnerId: request.OwnerId,
                RequesterId: request.RequesterId);

            var result = await _sender.Send(command, ct);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(new { error = result.Error });
        }

        /// Returns a paged list of messages for a chat thread (oldest-first).
        [HttpGet("{chatId:guid}/messages")]
        [ProducesResponseType(typeof(ConversationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMessages(
            Guid chatId,
            [FromQuery] Guid requestingUserId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken ct = default)
        {
            var query = new GetConversationQuery(chatId, requestingUserId, pageNumber, pageSize);
            var result = await _sender.Send(query, ct);

            return result.IsSuccess
                ? Ok(result.Value)
                : NotFound(new { error = result.Error });
        }

        /// Sends a message. Also pushes "ReceiveMessage" to all connected clients via SignalR.
        [HttpPost("{chatId:guid}/messages")]
        [ProducesResponseType(typeof(SendMessageResult), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendMessage(
            Guid chatId,
            [FromBody] SendMessageRequest request,
            CancellationToken ct)
        {
            var command = new SendMessageCommand(
                ChatId: chatId,
                SenderId: request.SenderId,
                Body: request.Body,
                Type: MessageType.Text);

            var result = await _sender.Send(command, ct);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetMessages), new { chatId }, result.Value)
                : BadRequest(new { error = result.Error });
        }

        /// Marks all messages in the chat as read for the given user.
        /// Pushes "MessagesRead" to all connected clients via SignalR.
        [HttpPost("{chatId:guid}/read")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MarkRead(
            Guid chatId,
            [FromBody] MarkReadRequest request,
            CancellationToken ct)
        {
            var command = new MarkMessagesReadCommand(chatId, request.ReaderId);
            var result = await _sender.Send(command, ct);

            return result.IsSuccess ? NoContent() : BadRequest(new { error = result.Error });
        }
    }
}
