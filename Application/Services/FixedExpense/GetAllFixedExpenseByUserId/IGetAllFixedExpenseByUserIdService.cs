using Application.Shared.Dtos;
using FluentResults;

namespace Application.Services.FixedExpense.GetAllFixedExpenseByUserId;

public interface IGetAllFixedExpenseByUserIdService
{
    Task<Result<IEnumerable<FixedExpenseResponseDto>>> ExecuteAsync(Guid userId, DateTime referenceDate);
}