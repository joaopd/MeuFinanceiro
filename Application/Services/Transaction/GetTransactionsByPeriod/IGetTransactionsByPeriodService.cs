using Application.Shared.Dtos;
using Application.Shared.Results;
using FluentResults;

namespace Application.Services.Transaction.GetTransactionsByPeriod;

public interface IGetTransactionsByPeriodService
{
    Task<Result<PaginatedResult<TransactionResponseDto>>> ExecuteAsync(
        GetTransactionsByPeriodRequest request);
}
