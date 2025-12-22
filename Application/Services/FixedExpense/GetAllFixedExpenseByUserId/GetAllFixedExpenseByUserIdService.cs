using Application.Shared.Dtos;
using Application.Shared.Mappers;
using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.FixedExpense.GetAllFixedExpenseByUserId;

public class GetAllFixedExpenseByUserIdService : IGetAllFixedExpenseByUserIdService
{
    private readonly IFixedExpenseRepository _fixedExpense;
    private readonly ILogger<GetAllFixedExpenseByUserIdService> _logger;

    public GetAllFixedExpenseByUserIdService(
        IFixedExpenseRepository fixedExpense,
        ILogger<GetAllFixedExpenseByUserIdService> logger)
    {
        _fixedExpense = fixedExpense;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<FixedExpenseResponseDto>>> ExecuteAsync(Guid userId, DateTime referenceDate)
    {
        try
        {
            var fixedExpenses = await _fixedExpense.GetActiveAsync(userId, referenceDate);
            
            if (fixedExpenses is null)
                return Result.Fail(FinanceErrorMessage.FixedExpenseNotFound);
            
            return Result.Ok(fixedExpenses.Select(c => c.ToDto()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dependents for user {userId}", userId);
            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }
}