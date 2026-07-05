using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.ItemAggregates;
using UniCare.Domain.Aggregates.UserAggregates;
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

        public async Task<Item?> GetItemWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(i => i.Owner)
                .Include(i => i.Category)
                .Include(i => i.FavoritedBy)
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
                }
        public async Task<(List<Item> Items, int TotalCount)> GetPagedAsync(
        public async Task<(List<Item> Items, int TotalCount, HashSet<Guid> FavoritedItemIds)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        ItemType? itemType,
        ItemStatus? excludeStatus,
        Guid? currentUserId,
        Guid? categoryId,
        bool? isFree,
        bool? availableOnly,
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

            if (itemType.HasValue)
            {
                query = query.Where(i => i.ItemType == itemType.Value);
            }
            if (categoryId.HasValue)
            {
                query = query.Where(i => i.CategoryId == categoryId.Value);
            }
            if (isFree == true)
            {
                query = query.Where(i => EF.Property<decimal>(i.Price, "Amount") == 0m);
            }
            if (availableOnly.HasValue) {
                
                query = query.Where(i => i.Status == ItemStatus.Available);
            }
            var orderedQuery = query.OrderByDescending(i => i.CreatedAt);
            var totalCount = await orderedQuery.CountAsync(cancellationToken);

            var items = await orderedQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var favoritedItemIds = new HashSet<Guid>();

            if (currentUserId.HasValue && items.Count > 0)
            {
                var itemIds = items.Select(i => i.Id).ToList();

                favoritedItemIds = (await _context.Set<UserFavorite>()
                    .AsNoTracking()
                    .Where(f => f.UserId == currentUserId.Value && itemIds.Contains(f.ItemId))
                    .Select(f => f.ItemId)
                    .ToListAsync(cancellationToken))
                    .ToHashSet();
            }

            return (items, totalCount, favoritedItemIds);
        }
        public async Task<(Item? Item, bool IsFavorited)> GetByIdAsync(
        Guid itemId,
        Guid? currentUserId,
        CancellationToken cancellationToken = default)
        {
            var item = await _dbSet
                .AsNoTracking()
                .Include(i => i.Owner)
                .Include(i => i.Category)
                .FirstOrDefaultAsync(i => i.Id == itemId, cancellationToken);

            if (item == null)
            {
                return (null, false);
            }

            bool isFavorited = false;

            if (currentUserId.HasValue)
            {
                isFavorited = await _context.Set<UserFavorite>()
                    .AsNoTracking()
                    .AnyAsync(f => f.UserId == currentUserId.Value && f.ItemId == itemId, cancellationToken);
            }

            return (item, isFavorited);
        }
    }
}
