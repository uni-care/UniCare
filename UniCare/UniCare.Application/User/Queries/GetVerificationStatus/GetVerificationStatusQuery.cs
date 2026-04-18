using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.User.DTOs.Verification;

namespace UniCare.Application.User.Queries.GetVerificationStatus
{
    public class GetVerificationStatusQuery : IRequest<Result<VerificationStatusDto>>
    {
        public Guid UserId { get; set; }
    }

}
