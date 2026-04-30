using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;

namespace UniCare.Application.Chats.Commands.MarkMessagesRead
{
    public sealed record MarkMessagesReadCommand(
          Guid ChatId,
          Guid ReaderId
      ) : ICommand<Result<bool>>;
}
