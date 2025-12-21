using System.Data;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infra.Database;

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException("Connection string not found");
    }

    public IDbConnection CreateConnection()
        => new NpgsqlConnection(_connectionString);
}