using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.UserAggregates;
using UniCare.Domain.Interfaces;

namespace UniCare.Infrastructure.Persistence.Repositories
{
    public class StudentVerificationRepository : IStudentVerificationRepository
    {
        private readonly IApplicationDbContext _dbContext;

        public StudentVerificationRepository(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<StudentVerification?> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.StudentVerifications
                .FirstOrDefaultAsync(sv => sv.UserId == userId, cancellationToken);
        }

        public void Add(StudentVerification verification)
        {
            _dbContext.StudentVerifications.Add(verification);
        }
    }
}
