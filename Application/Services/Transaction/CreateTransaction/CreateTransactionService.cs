using Domain.Abstractions.ErrorHandling;
using Domain.InterfaceRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Transaction.CreateTransaction;

public class CreateTransactionService : ICreateTransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<CreateTransactionService> _logger;

    public CreateTransactionService(
        ITransactionRepository transactionRepository,
        ILogger<CreateTransactionService> logger)
    {
        _transactionRepository = transactionRepository;
        _logger = logger;
    }

    public async Task<Result<bool>> ExecuteAsync(CreateTransactionRequestDto request)
    {
        try
        {
            _logger.LogInformation(
                "CreateTransaction started - UserId: {UserId}, Amount: {Amount}, Installments: {Installments}",
                request.UserId,
                request.Amount,
                request.TotalInstallments);
            
            if (request.Amount <= 0)
                return Result.Fail(FinanceErrorMessage.InvalidTransactionAmount);

            if (request.CardId is not null && request.PaymentMethod is null)
                return Result.Fail(FinanceErrorMessage.PaymentMethodRequiredForCardTransaction);

            if (request.TotalInstallments < 1)
                request.TotalInstallments = 1;

            var transactions = new List<Domain.Entities.Transaction>();
            for (int i = 0; i < request.TotalInstallments; i++)
            {
                var transactionDate = request.TransactionDate.AddMonths(i);

                // No loop de criação:
                var transaction = new Domain.Entities.Transaction(
                    request.UserId,
                    request.CategoryId,
                    request.Amount,
                    transactionDate,
                    request.TransactionType,
                    request.CardId,
                    request.PaymentMethod,
                    i + 1,
                    request.TotalInstallments,
                    request.IsFixed,
                    false,
                    null,
                    request.Observation
                );

                transactions.Add(transaction);
            }
            
            var insertSuccess = await _transactionRepository.InsertBulkAsync(transactions);

            _logger.LogInformation(
                "CreateTransaction finished successfully - UserId: {UserId}, TotalTransactions: {Count}",
                request.UserId,
                transactions.Count);

            return Result.Ok(insertSuccess);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating transactions for UserId: {UserId}", request.UserId);
            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }
}
