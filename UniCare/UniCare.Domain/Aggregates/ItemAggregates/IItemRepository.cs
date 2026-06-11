using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Common;

namespace UniCare.Domain.Aggregates.ItemAggregates
{
    public interface IItemRepository : IBaseRepository<Item>
    {
        Task<List<Item>> GetAvailableItemsAsync(CancellationToken cancellationToken = default);
        Task<Item?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
