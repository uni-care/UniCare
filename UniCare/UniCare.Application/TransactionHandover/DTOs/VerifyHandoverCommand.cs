using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.TransactionHandover;

namespace UniCare.Application.TransactionHandover.DTOs
{
    public record VerifyHandoverCommand(
     Guid TransactionId,
     HandoverType Type,
     Guid VerifyingUserId,
     string RawPin              // Entered manually OR decoded from QR
 );

}
