using Application.Shared.Dtos;
using Application.Shared.Mappers;
using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Category.GetAllCategories;

public class GetAllCategoriesService(
    ICategoryRepository categoryRepository,
    ILogger<GetAllCategoriesService> logger
    ) : IGetAllCategoriesService
{
    public async Task<Result<IEnumerable<CategoryResponseDto>>> ExecuteAsync()
    {
        try
        {
            var categories = await categoryRepository.GetAllAsync();

            var response = categories
                .Select(c => c.ToDto())
                .ToList();

            return Result.Ok((IEnumerable<CategoryResponseDto>)response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting all categories");
            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }
}