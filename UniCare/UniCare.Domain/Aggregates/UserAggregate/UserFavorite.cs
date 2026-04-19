using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.ItemAggregates;
using UniCare.Domain.Common;

namespace UniCare.Domain.Aggregates.UserAggregate
{
    public class UserFavorite : BaseEntity
    {
        public Guid UserId { get; private set; }
        public Guid ItemId { get; private set; }

        // Navigation properties
        public virtual User User { get; private set; }
        public virtual Item Item { get; private set; }

        private UserFavorite() { } // EF Core

        public static UserFavorite Create(Guid userId, Guid itemId)
        {
            return new UserFavorite
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ItemId = itemId,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
