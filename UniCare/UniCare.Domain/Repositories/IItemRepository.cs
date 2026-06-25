using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.ItemAggregates;
using UniCare.Domain.Common;

namespace UniCare.Domain.Repositories
{
    public interface IItemRepository : IBaseRepository<Item>
    {
        Task<List<Item>> GetAvailableItemsAsync(CancellationToken cancellationToken = default);
        Task<Item?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
        //to get paged items with total count
        Task<(List<Item> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);
    }
}
