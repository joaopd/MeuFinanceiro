using FluentResults;

namespace Application.Services.Transaction.CreateTransaction;

public interface ICreateTransactionService
{
    Task<Result<bool>> ExecuteAsync(CreateTransactionRequestDto request);
}