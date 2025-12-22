using Domain.Abstractions.ErrorHandling;
using Domain.Enums;
using Domain.InterfaceRepository;
using Domain.Records;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace Application.Services.Dashboard.GetDashboardService;

public class GetDashboardService(
    ITransactionRepository transactionRepository,
    ILogger<GetDashboardService> logger
    ) : IGetDashboardService
{
    public async Task<Result<GetDashboardResponse>> ExecuteAsync(
        Guid userId, 
        DateTime startDate, 
        DateTime endDate)
    {
        try
        {
            logger.LogInformation("GetDashboard started...");

            if (startDate > endDate)
                return Result.Fail(FinanceErrorMessage.InvalidPeriod);
            
            var balanceTask = transactionRepository.GetBalanceByPeriodAsync(userId, startDate, endDate);
            var expensesTask = transactionRepository.GetExpensesByCategoryAsync(userId, startDate, endDate);
            var cashFlowTask = transactionRepository.GetCashFlowAsync(userId, startDate, endDate);
            
            var paidExpensesTask = transactionRepository.GetPaidExpensesAmountAsync(userId, startDate, endDate);

            await Task.WhenAll(balanceTask, expensesTask, cashFlowTask, paidExpensesTask);

            
            var cashFlowRecords = cashFlowTask.Result.ToArray();
            var income = cashFlowRecords.FirstOrDefault(x => x.TransactionType == TransactionType.INCOME)?.Total ?? 0;
            var expense = Math.Abs(cashFlowRecords.FirstOrDefault(x => x.TransactionType == TransactionType.EXPENSE)?.Total ?? 0);

            
            var response = new GetDashboardResponse
            {
                Balance = income - expense, 
                TotalIncome = income,
                TotalExpense = expense,
                TotalPaid = paidExpensesTask.Result, 
                
                ExpensesByCategory = expensesTask.Result
                    .Select(e => new ChartItem { Name = e.Category, Value = e.Total, Type = "Expense" })
                    .ToList(),
                IncomesAndExpenses = new List<ChartItem> 
                { 
                    new() { Name = "Entradas", Value = income, Type = "Income" },
                    new() { Name = "Saídas", Value = expense, Type = "Expense" }
                }
            };

            return Result.Ok(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting dashboard");
            return Result.Fail(FinanceErrorMessage.DatabaseError);
        }
    }
}