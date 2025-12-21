using Domain.InterfaceRepository;
using FluentMigrator.Runner; // <--- Importante
using Infra.Database;
using Infra.Repository;
using Microsoft.Extensions.Configuration; // <--- Importante
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IFixedExpenseRepository, FixedExpenseRepository>();

        // ===========================================
        // CONFIGURAÇÃO DO FLUENT MIGRATOR
        // ===========================================
        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(configuration.GetConnectionString("DefaultConnection"))
                .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole());

        return services;
    }
}