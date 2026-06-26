using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Domain.Aggregates.UserAggregates
{
    public interface IStudentVerificationRepository
    {
        Task<StudentVerification?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        void Add(StudentVerification verification);
    }
}
