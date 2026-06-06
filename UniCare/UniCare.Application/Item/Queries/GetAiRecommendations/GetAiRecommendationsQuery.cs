using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Item.DTOs;

namespace UniCare.Application.Item.Queries.GetAiRecommendations
{
    public record GetAiRecommendationsQuery(string Prompt) : IRequest<List<ItemDto>>;
}
