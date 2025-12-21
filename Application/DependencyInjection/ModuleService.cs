using Application.Services.Transaction.CreateTransaction;
using Application.Services.Transaction.GetTransactionsByPeriod;
using Application.Services.Transaction.UpdateTransaction;
using Application.Services.User.CreateUser;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DependencyInjection;

public static class ModuleService
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICreateUserService, CreateUserService>();
        services.AddScoped<ICreateTransactionService, CreateTransactionService>();
        services.AddScoped<IUpdateTransactionService, UpdateTransactionService>();
        services.AddScoped<IGetTransactionsByPeriodService, GetTransactionsByPeriodService>();

        
        return services;
    }
}