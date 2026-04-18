using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.TransactionHandover;

namespace UniCare.Application.TransactionHandover.DTOs
{
    public record GenerateHandoverResponse(
    Guid HandoverId,
    string Pin,              // Raw 6-digit PIN — shown once to the receiver
    string QrPayload,        
    HandoverType Type,
    DateTime ExpiresAt
);
}
