using Application.Shared.Dtos;
using FluentResults;

namespace Application.Services.Category.GetAllCategories;

public interface IGetAllCategoriesService
{
    Task<Result<IEnumerable<CategoryResponseDto>>> ExecuteAsync();
}