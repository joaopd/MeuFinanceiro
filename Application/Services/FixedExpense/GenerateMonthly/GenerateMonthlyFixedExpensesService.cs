using Domain.InterfaceRepository;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.FixedExpense.GenerateMonthly;

public class GenerateMonthlyFixedExpensesService : IGenerateMonthlyFixedExpensesService
{
    private readonly IFixedExpenseRepository _fixedRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<GenerateMonthlyFixedExpensesService> _logger;

    public GenerateMonthlyFixedExpensesService(
        IFixedExpenseRepository fixedRepository,
        ITransactionRepository transactionRepository,
        ILogger<GenerateMonthlyFixedExpensesService> logger)
    {
        _fixedRepository = fixedRepository;
        _transactionRepository = transactionRepository;
        _logger = logger;
    }

    public async Task<Result<int>> ExecuteAsync(Guid userId, int month, int year)
    {
        try
        {
            _logger.LogInformation(
                "GenerateMonthlyFixedExpenses started - UserId: {UserId}, Period: {Month}/{Year}",
                userId, month, year);

            var referenceDate = new DateTime(year, month, 1);
            var activeFixedExpenses = await _fixedRepository.GetActiveAsync(userId, referenceDate);

            var newTransactions = new List<Domain.Entities.Transaction>();
            var count = 0;

            foreach (var fixedItem in activeFixedExpenses)
            {
                var alreadyGenerated = await _transactionRepository
                    .ExistsByFixedExpenseAsync(fixedItem.Id, month, year);

                if (alreadyGenerated)
                    continue;

                var day = Math.Min(
                    fixedItem.StartDate.Day,
                    DateTime.DaysInMonth(year, month));

                var transactionDate = new DateTime(year, month, day);

                var transaction = new Domain.Entities.Transaction(
                    userId: userId,
                    categoryId: fixedItem.CategoryId,
                    amount: fixedItem.Amount,
                    transactionDate: transactionDate,
                    transactionType: Domain.Enums.TransactionType.EXPENSE,

                    // 🔒 REGRA: conta fixa NÃO é cartão
                    cardId: null,
                    paymentMethod: Domain.Enums.PaymentMethod.CASH,
                    invoiceId: null,

                    installmentNumber: 1,
                    totalInstallments: 1,
                    isFixed: true,
                    isPaid: false,
                    fixedExpenseId: fixedItem.Id,
                    observation: fixedItem.Description
                );

                newTransactions.Add(transaction);
                count++;
            }

            if (newTransactions.Any())
                await _transactionRepository.InsertBulkAsync(newTransactions);

            _logger.LogInformation(
                "GenerateMonthlyFixedExpenses finished - Generated: {Count}",
                count);

            return Result.Ok(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating monthly fixed expenses");
            return Result.Fail(
                Domain.Abstractions.ErrorHandling.FinanceErrorMessage.DatabaseError);
        }
    }
}
