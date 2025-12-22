using Application.Shared.Dtos;
using FluentResults;

namespace Application.Services.Category.CreateCategory;

public interface ICreateCategoryService
{
    Task<Result<CategoryResponseDto>> ExecuteAsync(CreateCategoryRequestDto request);
}