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
            if (availableOnly.HasValue)
            {

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
        public async Task<(List<UserFavorite> Favorites, int TotalCount)> GetFavoritesPagedAsync(
        Guid userId,
        Guid? categoryId,
        string sortBy,
        bool descending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
        {
            IQueryable<UserFavorite> query = _context.UserFavorites
                .AsNoTracking()
                .Include(uf => uf.Item)
                    .ThenInclude(i => i.Owner)
                .Include(uf => uf.Item)
                    .ThenInclude(i => i.Category)
                .Where(uf => uf.UserId == userId);

            if (categoryId.HasValue)
            {
                query = query.Where(uf => uf.Item.CategoryId == categoryId);
            }

            query = (sortBy.ToLowerInvariant(), descending) switch
            {
                ("price", false) => query.OrderBy(uf => uf.Item.Price.Amount),
                ("price", true) => query.OrderByDescending(uf => uf.Item.Price.Amount),
                ("name", false) => query.OrderBy(uf => uf.Item.Title),
                ("name", true) => query.OrderByDescending(uf => uf.Item.Title),
                ("dateadded", false) => query.OrderBy(uf => uf.CreatedAt),
                _ => query.OrderByDescending(uf => uf.CreatedAt)
            };

            var totalCount = await query.CountAsync(cancellationToken);

            var favorites = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (favorites, totalCount);
        }
        public async Task<List<Item>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(i => i.Owner)
                .Include(i => i.Category)
                .Where(i => ids.Contains(i.Id))
                .ToListAsync(cancellationToken);
        }

        public async Task<UserFavorite?> GetFavoriteAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
        {
            return await _context.UserFavorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ItemId == itemId, cancellationToken);
        }

        public async Task AddFavoriteAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
        {
            var favorite = UserFavorite.Create(userId, itemId);
            _context.UserFavorites.Add(favorite);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RemoveFavoriteAsync(Guid userId, Guid itemId, CancellationToken cancellationToken = default)
        {
            var favorite = await _context.UserFavorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ItemId == itemId, cancellationToken);
            if (favorite != null)
            {
                _context.UserFavorites.Remove(favorite);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
