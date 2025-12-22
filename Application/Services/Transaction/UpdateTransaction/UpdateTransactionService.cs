using Application.Shared.Dtos;
using Application.Shared.Mappers;
using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Transaction.UpdateTransaction;

public class UpdateTransactionService(
    ITransactionRepository transactionRepository,
    ILogger<UpdateTransactionService> logger)
    : IUpdateTransactionService
{
    public async Task<Result<TransactionResponseDto>> ExecuteAsync(
        UpdateTransactionRequestDto request)
    {
        try
        {
            logger.LogInformation(
                "UpdateTransaction started - TransactionId: {TransactionId}",
                request.TransactionId);

            var transaction =
                await transactionRepository.GetByIdAsync(request.TransactionId);

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
            
            if (request.Observation != null)
            {
                transaction.UpdateObservation(request.Observation, request.UpdatedBy);
            }
            
            await transactionRepository.UpdateAsync(transaction);

            logger.LogInformation(
                "UpdateTransaction finished successfully - TransactionId: {TransactionId}",
                transaction.Id);

            return Result.Ok(transaction.ToDto());
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error while updating transaction - TransactionId: {TransactionId}",
                request.TransactionId);

            return Result.Fail(
                FinanceErrorMessage.DatabaseError);
        }
    }
}
