using Application.Shared.Dtos;
using FluentResults;

namespace Application.Services.Transaction.UpdateTransaction;

public interface IUpdateTransactionService
{
    Task<Result<TransactionResponseDto>> ExecuteAsync(
        UpdateTransactionRequestDto request);
}