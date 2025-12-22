using Application.Shared.Dtos;
using Domain.Entities;

namespace Application.Shared.Mappers;

public static class CategoryMapper
{
    public static CategoryResponseDto ToDto(this Category category)
        => new CategoryResponseDto(category.Id, category.Name);
}