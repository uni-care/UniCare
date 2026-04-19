using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Application.Item.DTOs
{
    public record ItemDto(
     Guid Id,
     string Name,
     string Description,
     decimal Price,
     int Quantity,
     bool IsAvailable,
     DateTime CreatedAt,
     DateTime UpdatedAt
    );
}
