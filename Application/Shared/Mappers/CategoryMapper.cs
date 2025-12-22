using Application.Shared.Dtos;
using Domain.Entities;

namespace Application.Shared.Mappers;

public static class CategoryMapper
{
    public static CategoryResponseDto ToDto(this Category category)
    {
        return new CategoryResponseDto
        {
            Id = category.Id,
            Name = category.Name,
        };
    }
}