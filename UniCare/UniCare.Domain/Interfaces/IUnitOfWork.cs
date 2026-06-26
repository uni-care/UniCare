using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.UserAggregates;

namespace UniCare.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IStudentVerificationRepository StudentVerifications { get; }
        Task<int> CommitAsync(CancellationToken cancellationToken = default);
    }
}
