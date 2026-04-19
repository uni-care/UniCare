using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Domain.Aggregates.TransactionHandover
{
    public enum HandoverStatus
    {
        Pending = 1,
        Verified = 2,
        Expired = 3
    }
}
