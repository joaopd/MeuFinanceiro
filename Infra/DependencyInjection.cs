using Domain.InterfaceRepository;
using FluentMigrator.Runner; // <--- Importante
using Infra.Database;
using Infra.Repository;
using Microsoft.Extensions.Configuration; // <--- Importante
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Domain.InterfaceRepository.BaseRepository;

namespace Infra;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IFixedExpenseRepository, FixedExpenseRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICardRepository, CardRepository>();

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