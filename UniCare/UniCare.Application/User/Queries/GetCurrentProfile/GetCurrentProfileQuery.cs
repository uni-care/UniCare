using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.User.DTOs.Profile;

namespace UniCare.Application.User.Queries.GetCurrentProfile
{
    public class GetCurrentProfileQuery : IRequest<Result<UserProfileDto>>
    {
        public Guid UserId { get; set; }
    }
}
