using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniCare.Application.Common
{
    public record AiRecommendationResponse(List<Guid> ItemIds);
}
