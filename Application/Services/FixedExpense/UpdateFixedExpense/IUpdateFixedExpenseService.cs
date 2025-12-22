using Application.Shared.Dtos;
using FluentResults;

namespace Application.Services.FixedExpense.UpdateFixedExpense;

public interface IUpdateFixedExpenseService
{
    Task<Result<FixedExpenseResponseDto>> ExecuteAsync(UpdateFixedExpenseRequestDto request);
}