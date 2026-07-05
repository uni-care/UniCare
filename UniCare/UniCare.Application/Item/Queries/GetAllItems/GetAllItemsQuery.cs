using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Item.DTOs;
using UniCare.Domain.Enums;

namespace UniCare.Application.Item.Queries.GetAllItems
{
    public class GetAllItemsQuery : PaginationParams, IRequest<PaginatedList<ItemDto>>
    {
        public Guid? CurrentUserId { get; set; }
        public ItemType? ItemType { get; set; } 
        public Guid? CategoryId { get; set; }
        public bool? IsFree { get; set; }
        public bool? AvailableOnly { get; set; } 
     }

}
