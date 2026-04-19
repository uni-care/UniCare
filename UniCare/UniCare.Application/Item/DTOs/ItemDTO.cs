using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Application.Item.DTOs
{
    public record ItemDto(
    Guid Id,
    string Title,
    string Description,
    decimal Price,
    string Currency,
    string Status,
    Guid OwnerId,
    string OwnerName,
    Guid CategoryId,
    string CategoryName,
    DateTime? AvailableFrom,
    DateTime? AvailableTo,
    string? Location,
    List<string> ImageUrls,
    bool IsFavorited,
    int FavoriteCount,
    DateTime CreatedAt,
    DateTime UpdatedAt
    );
}
