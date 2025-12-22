using Application.Shared.Dtos;
using Application.Shared.Mappers;
using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.FixedExpense.GetAllFixedExpenseByUserId;

public class GetAllFixedExpenseByUserIdService(
    IFixedExpenseRepository fixedExpense,
    ILogger<GetAllFixedExpenseByUserIdService> logger) : IGetAllFixedExpenseByUserIdService
{
   public async Task<Result<IEnumerable<FixedExpenseResponseDto>>> ExecuteAsync(Guid userId, DateTime referenceDate)
    {
        try
        {
            var fixedExpenses = await fixedExpense.GetActiveAsync(userId, referenceDate);

            return Result.Ok(fixedExpenses.Select(c => c.ToDto()));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting dependents for user {userId}", userId);
            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }
}