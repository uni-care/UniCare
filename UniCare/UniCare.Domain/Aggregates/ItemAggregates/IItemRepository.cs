using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Common;
using UniCare.Domain.Enums;

namespace UniCare.Domain.Aggregates.ItemAggregates
{
    public interface IItemRepository : IBaseRepository<Item>
    {
        Task<List<Item>> GetAvailableItemsAsync(CancellationToken cancellationToken = default);
        Task<Item?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        Task<Item?> GetItemWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        //to get paged items with total count
        Task<(List<Item> Items, int TotalCount, HashSet<Guid> FavoritedItemIds)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        ItemType? itemType,
        ItemStatus? excludeStatus,
        Guid? currentUserId,
        Guid? categoryId,
        bool? isFree,
        bool? availableOnly,
        CancellationToken cancellationToken = default);
        Task<(Item? Item, bool IsFavorited)> GetByIdAsync(
        Guid itemId,
        Guid? currentUserId,
        CancellationToken cancellationToken = default);
    }
}
