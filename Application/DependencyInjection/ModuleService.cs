using Application.Services.Card.CreateCard;
using Application.Services.Card.DeleteCard;
using Application.Services.Card.GetFamilyCards;
using Application.Services.Card.GetUserCards;
using Application.Services.Card.UpdateCard;
using Application.Services.Category.CreateCategory;
using Application.Services.Category.GetAllCategories;
using Application.Services.Dashboard.GetDashboardService;
using Application.Services.FixedExpense.CreateFixedExpense;
using Application.Services.FixedExpense.GenerateMonthly;
using Application.Services.FixedExpense.GetAllFixedExpenseByUserId;
using Application.Services.FixedExpense.UpdateFixedExpense;
using Application.Services.Transaction.CreateTransaction;
using Application.Services.Transaction.GetTransactionsByPeriod;
using Application.Services.Transaction.ImportInvoice;
using Application.Services.Transaction.TogglePaymentStatus;
using Application.Services.Transaction.UpdateTransaction;
using Application.Services.User.CreateUser;
using Application.Services.User.GetByEmail;
using Application.Services.User.GetDependents;
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
        services.AddScoped<IGetByEmailService, GetByEmailService>();
        services.AddScoped<IGetDependentsService, GetDependentsService>();


        // =========================
        // TRANSACTION
        // =========================
        services.AddScoped<ICreateTransactionService, CreateTransactionService>();
        services.AddScoped<IUpdateTransactionService, UpdateTransactionService>();
        services.AddScoped<IGetTransactionsByPeriodService, GetTransactionsByPeriodService>();
        services.AddScoped<ITogglePaymentStatusService, TogglePaymentStatusService>();
        services.AddScoped<IImportInvoiceService, GeminiInvoiceService>();

        // =========================
        // FIXED EXPENSE
        // =========================
        services.AddScoped<ICreateFixedExpenseService, CreateFixedExpenseService>();
        services.AddScoped<IGenerateMonthlyFixedExpensesService, GenerateMonthlyFixedExpensesService>();
        services.AddScoped<IUpdateFixedExpenseService, UpdateFixedExpenseService>();
        services.AddScoped<IGetAllFixedExpenseByUserIdService, GetAllFixedExpenseByUserIdService>();

        // =========================
        // DASHBOARD
        // =========================
        services.AddScoped<IGetDashboardService, GetDashboardService>();
        
        // =========================
        // Category
        // =========================
        services.AddScoped<ICreateCategoryService, CreateCategoryService>();
        services.AddScoped<IGetAllCategoriesService, GetAllCategoriesService>(); 
        
        // =========================
        // Services de Card
        // =========================
        services.AddScoped<ICreateCardService, CreateCardService>();
        services.AddScoped<IGetFamilyCardsService, GetFamilyCardsService>();
        services.AddScoped<IGetUserCardsService, GetUserCardsService>();
        services.AddScoped<IUpdateCardService, UpdateCardService>();
        services.AddScoped<IDeleteCardService, DeleteCardService>();
        
        return services;
    }
}