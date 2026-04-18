using UniCare.Domain.Aggregates.TransactionAggregate;

namespace UniCare.Api.Controllers.Transaction.Requests
{
    public record CreateTransactionRequest(
         Guid ItemId,
         Guid OwnerId,
         Guid RequesterId,
         TransactionType Type,
         decimal AgreedPrice,
         DateTime? RentalReturnDue
     );
}