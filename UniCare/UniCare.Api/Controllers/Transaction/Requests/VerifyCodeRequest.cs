namespace UniCare.Api.Controllers.Transaction.Requests
{
    public record VerifyCodeRequest(
         Guid VerifyingUserId,
         string Pin
     );
}
