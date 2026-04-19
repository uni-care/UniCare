using Microsoft.AspNetCore.SignalR;
using UniCare.Domain.Aggregates.ChatAggregate;
using UniCare.Infrastructure.Hubs;

namespace UniCare.Infrastructure.Services
{
    public class SignalRChatNotificationService : IChatNotificationService
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public SignalRChatNotificationService(IHubContext<ChatHub> hubContext)
            => _hubContext = hubContext;

        /// Client-side event: "ReceiveMessage"
        public async Task NotifyMessageSentAsync(
            Guid chatId,
            MessageDto message,
            CancellationToken ct = default)
        {
            await _hubContext.Clients
                .Group(ChatHub.GroupName(chatId.ToString()))
                .SendAsync("ReceiveMessage", message, ct);
        }

        /// Client-side event: "MessagesRead"
        public async Task NotifyMessagesReadAsync(
            Guid chatId,
            Guid readerId,
            CancellationToken ct = default)
        {
            await _hubContext.Clients
                .Group(ChatHub.GroupName(chatId.ToString()))
                .SendAsync("MessagesRead", new { chatId, readerId }, ct);
        }
    }
}
