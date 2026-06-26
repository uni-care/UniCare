using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Item.DTOs;
using UniCare.Application.Common;

namespace UniCare.Application.Item.Queries.GetAllItems
{
    public class GetAllItemsQuery : PaginationParams, IRequest<PaginatedList<ItemDto>>
    { 
    }

}
