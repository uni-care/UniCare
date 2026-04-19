using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;

namespace UniCare.Application.Chats.Commands.GetOrCreateChat
{
    public sealed record GetOrCreateChatCommand(
          Guid TransactionId,
          Guid OwnerId,
          Guid RequesterId
      ) : ICommand<Result<GetOrCreateChatResult>>;

    public sealed record GetOrCreateChatResult(
        Guid ChatId,
        Guid TransactionId,
        bool WasCreated
    );
}
