using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.ItemAggregates;
using UniCare.Domain.Enums;

namespace UniCare.Infrastructure.Persistence.Repositories
{
    public class ItemRepository : BaseRepository<Item>, IItemRepository
    {
        public ItemRepository(UniCareDbContext context) : base(context)
        {
        }

        public override async Task<List<Item>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(i => i.Owner)
                .Include(i => i.Category)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Item>> GetAvailableItemsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(i => i.Category)
                .Where(i => i.Status == ItemStatus.Available)
                .ToListAsync(cancellationToken);
        }

        public async Task<Item?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Title == name, cancellationToken);
        }
        public async Task<(List<Item> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        ItemStatus? excludeStatus,
        CancellationToken cancellationToken = default)
        {
            IQueryable<Item> query = _dbSet
                .AsNoTracking()
                .Include(i => i.Owner)
                .Include(i => i.Category);

            if (excludeStatus.HasValue)
            {
                query = query.Where(i => i.Status != excludeStatus.Value);
            }

            var orderedQuery = query.OrderByDescending(i => i.CreatedAt);
            var totalCount = await orderedQuery.CountAsync(cancellationToken);

            var items = await orderedQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);


            return (items, totalCount);
        }
    }
}
