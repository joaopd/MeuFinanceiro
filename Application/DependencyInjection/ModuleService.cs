using Application.Services.Dashboard.GetDashboardService;
using Application.Services.FixedExpense.CreateFixedExpense;
using Application.Services.FixedExpense.GenerateMonthly;
using Application.Services.Transaction.CreateTransaction;
using Application.Services.Transaction.GetTransactionsByPeriod;
using Application.Services.Transaction.TogglePaymentStatus;
using Application.Services.Transaction.UpdateTransaction;
using Application.Services.User.CreateUser;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // =========================
        // USER
        // =========================
        services.AddScoped<ICreateUserService, CreateUserService>();

        // =========================
        // TRANSACTION
        // =========================
        services.AddScoped<ICreateTransactionService, CreateTransactionService>();
        services.AddScoped<IUpdateTransactionService, UpdateTransactionService>();
        services.AddScoped<IGetTransactionsByPeriodService, GetTransactionsByPeriodService>();
        services.AddScoped<ITogglePaymentStatusService, TogglePaymentStatusService>();

        // =========================
        // FIXED EXPENSE
        // =========================
        services.AddScoped<ICreateFixedExpenseService, CreateFixedExpenseService>();
        services.AddScoped<IGenerateMonthlyFixedExpensesService, GenerateMonthlyFixedExpensesService>();

        // =========================
        // DASHBOARD
        // =========================
        services.AddScoped<IGetDashboardService, GetDashboardService>();

        return services;
    }
}