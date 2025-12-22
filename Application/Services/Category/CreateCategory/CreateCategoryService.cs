using Application.Shared.Dtos;
using Application.Shared.Mappers;
using Domain.Abstractions.ErrorHandling;
using Domain.Entities;
using Domain.InterfaceRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Category.CreateCategory;

public class CreateCategoryService(
    ICategoryRepository categoryRepository,
    ILogger<CreateCategoryService> logger
    ) : ICreateCategoryService
{
    public async Task<Result<CategoryResponseDto>> ExecuteAsync(CreateCategoryRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result.Fail(FinanceErrorMessage.InvalidCategoryData);

            var category = new Domain.Entities.Category(request.Name);

            await categoryRepository.InsertAsync(category);

            return Result.Ok(category.ToDto());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating category {Name}", request.Name);
            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }
}