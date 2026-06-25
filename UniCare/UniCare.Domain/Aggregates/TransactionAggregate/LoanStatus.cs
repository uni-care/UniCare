using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Domain.Aggregates.TransactionAggregate
{
  
    public enum LoanStatus
    {
        /// Transaction is pending owner approval.
        PendingApproval = 1,

        /// Transaction approved, awaiting physical handover.
        AwaitingHandover = 2,

        ///Item has been handed over and is currently with the borrower.
        Active = 3,

        /// The rental due date has passed and the item has not been returned.
        Overdue = 4,

        /// Item has been returned and transaction is complete.
        Returned = 5,

        /// Transaction was cancelled before completion.
        Cancelled = 6
    }
}
