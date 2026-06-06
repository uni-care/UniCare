using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.UserAggregates;
using UniCare.Domain.Common;
using UniCare.Domain.Enums;
using UniCare.Domain.VOs;

namespace UniCare.Domain.Aggregates.ItemAggregates
{
    public class Item : BaseEntity
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public Money Price { get; private set; }
        public ItemStatus Status { get; private set; }
        public Guid OwnerId { get; private set; }
        public Guid CategoryId { get; private set; }
        public DateTime? AvailableFrom { get; private set; }
        public DateTime? AvailableTo { get; private set; }
        public string? Location { get; private set; }
        public List<string> ImageUrls { get; private set; } = new();

        // Navigation properties
        public virtual User Owner { get; private set; }
        public virtual ICollection<UserFavorite> FavoritedBy { get; private set; } = new List<UserFavorite>();

        private Item() { } // EF Core

        public static Item Create(
            string title,
            string description,
            Money price,
            Guid ownerId,
            DateTime? availableFrom = null,
            DateTime? availableTo = null,
            string? location = null,
            List<string>? imageUrls = null)
        {
            var item = new Item
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                Price = price,
                Status = ItemStatus.Draft,
                OwnerId = ownerId,
                AvailableFrom = availableFrom,
                AvailableTo = availableTo,
                Location = location,
                ImageUrls = imageUrls ?? new List<string>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return item;
        }

        public void Update(
            string? title = null,
            string? description = null,
            Money? price = null,
            Guid? categoryId = null,
            DateTime? availableFrom = null,
            DateTime? availableTo = null,
            string? location = null,
            List<string>? imageUrls = null)
        {
            if (!string.IsNullOrWhiteSpace(title)) Title = title;
            if (!string.IsNullOrWhiteSpace(description)) Description = description;
            if (price != null) Price = price;
            if (categoryId.HasValue) CategoryId = categoryId.Value;
            if (availableFrom.HasValue) AvailableFrom = availableFrom;
            if (availableTo.HasValue) AvailableTo = availableTo;
            if (location != null) Location = location;
            if (imageUrls != null) ImageUrls = imageUrls;

            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateStatus(ItemStatus status)
        {
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Publish()
        {
            if (Status == ItemStatus.Draft)
            {
                Status = ItemStatus.Available;
                UpdatedAt = DateTime.UtcNow;
            }
        }
        public bool IsAvailable() => Status == ItemStatus.Available;

        public bool IsDraft() => Status == ItemStatus.Draft;

        public bool IsRented() => Status == ItemStatus.Rented;
    }
}
