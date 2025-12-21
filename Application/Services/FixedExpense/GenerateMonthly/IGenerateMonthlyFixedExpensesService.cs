using FluentResults;

namespace Application.Services.FixedExpense.GenerateMonthly;

public interface IGenerateMonthlyFixedExpensesService
{
    Task<Result<int>> ExecuteAsync(Guid userId, int month, int year);
}