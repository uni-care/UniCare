using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Domain.Aggregates.TransactionHandoverAggregate
{
    public enum HandoverType
    {
        SaleDelivery = 1,
        RentalStart = 2,
        RentalReturn = 3
    }
}
