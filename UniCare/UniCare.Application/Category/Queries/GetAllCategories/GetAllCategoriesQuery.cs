using MediatR;
using UniCare.Application.Category.DTOs;

namespace UniCare.Application.Category.Queries.GetAllCategories;

public record GetAllCategoriesQuery : IRequest<List<CategoryDto>>;
