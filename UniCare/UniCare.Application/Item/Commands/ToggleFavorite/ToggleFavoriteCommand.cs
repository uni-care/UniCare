using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Application.Item.Commands.ToggleFavorite
{
    public record ToggleFavoriteCommand(Guid ItemId, Guid UserId) : IRequest<bool>;

}
