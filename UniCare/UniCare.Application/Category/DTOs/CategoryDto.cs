namespace UniCare.Application.Category.DTOs;

public record CategoryDto(
    Guid Id,
    string Name,
    string? Description
);
