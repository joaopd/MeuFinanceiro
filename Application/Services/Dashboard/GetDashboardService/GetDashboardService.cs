using Domain.Abstractions.ErrorHandling;
using Domain.Enums;
using Domain.InterfaceRepository;
using Domain.Records;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Dashboard.GetDashboardService;

public class GetDashboardService : IGetDashboardService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ILogger<GetDashboardService> _logger;

    public GetDashboardService(
        ITransactionRepository transactionRepository,
        ILogger<GetDashboardService> logger)
    {
        _transactionRepository = transactionRepository;
        _logger = logger;
    }

    public async Task<Result<GetDashboardResponse>> ExecuteAsync(
        Guid userId, 
        DateTime startDate, 
        DateTime endDate)
    {
        try
        {
            _logger.LogInformation("GetDashboard started...");

            if (startDate > endDate)
                return Result.Fail(FinanceErrorMessage.InvalidPeriod);

            
            var balanceTask = _transactionRepository.GetBalanceByPeriodAsync(userId, startDate, endDate);

            
            var expensesTask = _transactionRepository.GetExpensesByCategoryAsync(userId, startDate, endDate);

            
            var cashFlowTask = _transactionRepository.GetCashFlowAsync(userId, startDate, endDate);

            
            await Task.WhenAll(balanceTask, expensesTask, cashFlowTask);

            var balance = balanceTask.Result;
            var expenses = expensesTask.Result;
            var cashFlow = cashFlowTask.Result;

            var cashFlowChart = new List<ChartItem>();

            var cashFlowRecords = cashFlow as CashFlowRecord[] ?? cashFlow.ToArray();
            var income = cashFlowRecords.FirstOrDefault(x => x.TransactionType == TransactionType.INCOME)?.Total ?? 0;
            cashFlowChart.Add(new ChartItem { Name = "Entradas", Value = income, Type = "Income" });

            var expense = Math.Abs(cashFlowRecords.FirstOrDefault(x => x.TransactionType == TransactionType.EXPENSE)?.Total ?? 0);
            cashFlowChart.Add(new ChartItem { Name = "Saídas", Value = expense, Type = "Expense" });

            var response = new GetDashboardResponse
            {
                Balance = balance,
                ExpensesByCategory = expenses
                    .Select(e => new ChartItem { Name = e.Category, Value = e.Total, Type = "Expense" })
                    .ToList(),
                IncomesAndExpenses = cashFlowChart
            };

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard");
            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }
}