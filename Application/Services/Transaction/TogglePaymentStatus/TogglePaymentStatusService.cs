using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Transaction.TogglePaymentStatus;

public class TogglePaymentStatusService(
    ITransactionRepository transactionRepository,
    ILogger<TogglePaymentStatusService> logger) : ITogglePaymentStatusService
{
   public async Task<Result> ExecuteAsync(Guid transactionId, Guid updatedBy)
    {
        try
        {
            logger.LogInformation(
                "TogglePaymentStatus started - TransactionId: {TransactionId}",
                transactionId);

            var transaction = await transactionRepository.GetByIdAsync(transactionId);

            if (transaction is null)
            {
                logger.LogWarning("Transaction not found - TransactionId: {TransactionId}", transactionId);
                return Result.Fail(FinanceErrorMessage.TransactionNotFound);
            }

            transaction.SetPaymentStatus(updatedBy);

            await transactionRepository.UpdateAsync(transaction);

            logger.LogInformation(
                "TogglePaymentStatus finished successfully - TransactionId: {TransactionId}",
                transactionId);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error while toggling payment status - TransactionId: {TransactionId}",
                transactionId);

            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }
}