using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Transaction.TogglePaymentStatus;

public class TogglePaymentStatusService : ITogglePaymentStatusService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<TogglePaymentStatusService> _logger;

    public TogglePaymentStatusService(
        ITransactionRepository transactionRepository,
        ILogger<TogglePaymentStatusService> logger)
    {
        _transactionRepository = transactionRepository;
        _logger = logger;
    }

    public async Task<Result> ExecuteAsync(Guid transactionId, bool isPaid, Guid updatedBy)
    {
        try
        {
            _logger.LogInformation(
                "TogglePaymentStatus started - TransactionId: {TransactionId}, NewStatus: {IsPaid}",
                transactionId,
                isPaid);

            var transaction = await _transactionRepository.GetByIdAsync(transactionId);

            if (transaction is null)
            {
                _logger.LogWarning("Transaction not found - TransactionId: {TransactionId}", transactionId);
                return Result.Fail(FinanceErrorMessage.TransactionNotFound);
            }

            transaction.SetPaymentStatus(isPaid, updatedBy);

            await _transactionRepository.UpdateAsync(transaction);

            _logger.LogInformation(
                "TogglePaymentStatus finished successfully - TransactionId: {TransactionId}",
                transactionId);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error while toggling payment status - TransactionId: {TransactionId}",
                transactionId);

            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }
}