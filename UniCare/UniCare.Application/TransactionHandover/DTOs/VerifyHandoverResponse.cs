using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Application.TransactionHandover.DTOs
{
    public record VerifyHandoverResponse(
    bool Success,
    string Message,
    DateTime? VerifiedAt
);
}
