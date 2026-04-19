using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Item.DTOs;

namespace UniCare.Application.Item.Queries.GetItemById
{
    public record GetItemByIdQuery(Guid ItemId, Guid? CurrentUserId = null) : IRequest<ItemDto>;

}
