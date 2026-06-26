using System;
using System.Threading;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.ItemAggregates;
using UniCare.Domain.Common;

namespace UniCare.Domain.Repositories
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
    }
}