using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Domain.Aggregates.TransactionAggregate
{
    public enum TransactionStatus
    {
        PendingApproval = 1,
        AwaitingHandover = 2,
        Active = 3,
        Completed = 4,
        Cancelled = 5
    }
}
