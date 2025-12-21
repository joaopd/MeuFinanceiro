using Application.Shared.Dtos;
using FluentResults;

namespace Application.Services.FixedExpense.CreateFixedExpense;

public interface ICreateFixedExpenseService
{
    Task<Result<FixedExpenseResponseDto>> ExecuteAsync(CreateFixedExpenseRequestDto request);
}