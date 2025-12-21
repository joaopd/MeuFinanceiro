using Application.Shared.Dtos;
using Application.Shared.Mappers;
using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Transaction.UpdateTransaction;

public class UpdateTransactionService : IUpdateTransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<UpdateTransactionService> _logger;

    public UpdateTransactionService(
        ITransactionRepository transactionRepository,
        ILogger<UpdateTransactionService> logger)
    {
        _transactionRepository = transactionRepository;
        _logger = logger;
    }

    public async Task<Result<TransactionResponseDto>> ExecuteAsync(
        UpdateTransactionRequestDto request)
    {
        try
        {
            _logger.LogInformation(
                "UpdateTransaction started - TransactionId: {TransactionId}",
                request.TransactionId);

            var transaction =
                await _transactionRepository.GetByIdAsync(request.TransactionId);

            if (transaction is null)
            {
                return Result.Fail(
                    FinanceErrorMessage.TransactionNotFound);
            }

            if (request.Amount.HasValue)
            {
                transaction.ChangeAmount(
                    request.Amount.Value,
                    request.UpdatedBy);
            }

            if (request.TransactionDate.HasValue)
            {
                transaction.ChangeDate(
                    request.TransactionDate.Value,
                    request.UpdatedBy);
            }

            await _transactionRepository.UpdateAsync(transaction);

            _logger.LogInformation(
                "UpdateTransaction finished successfully - TransactionId: {TransactionId}",
                transaction.Id);

            return Result.Ok(transaction.ToDto());
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error while updating transaction - TransactionId: {TransactionId}",
                request.TransactionId);

            return Result.Fail(
                FinanceErrorMessage.DatabaseError);
        }
    }
}
