using UniCare.Domain.Aggregates.ItemAggregates;
using UniCare.Domain.Repositories;

namespace UniCare.Infrastructure.Persistence.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(UniCareDbContext context) : base(context)
        {
        }
    }
}