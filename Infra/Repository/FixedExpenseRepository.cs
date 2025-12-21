using Dapper;
using Domain.Entities;
using Domain.InterfaceRepository;
using Infra.Database;
using Infra.Queries;

namespace Infra.Repository;

public class FixedExpenseRepository : IFixedExpenseRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public FixedExpenseRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<FixedExpense?> GetByIdAsync(Guid id)
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<FixedExpense>(
            FixedExpenseQueries.GetById,
            new { Id = id });
    }

    public async Task<IEnumerable<FixedExpense>> GetAllAsync()
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryAsync<FixedExpense>(
            FixedExpenseQueries.GetAll);
    }

    public async Task<IEnumerable<FixedExpense>> GetActiveAsync(Guid userId, DateTime referenceDate)
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryAsync<FixedExpense>(
            FixedExpenseQueries.GetActiveByUser,
            new { UserId = userId, ReferenceDate = referenceDate });
    }

    public async Task<Guid> InsertAsync(FixedExpense entity)
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QuerySingleAsync<Guid>(
            FixedExpenseQueries.Insert,
            entity);
    }

    public async Task UpdateAsync(FixedExpense entity)
    {
        using var conn = _connectionFactory.CreateConnection();
        await conn.ExecuteAsync(
            FixedExpenseQueries.Update,
            entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        using var conn = _connectionFactory.CreateConnection();
        await conn.ExecuteAsync(
            FixedExpenseQueries.SoftDelete,
            new { Id = id, UpdatedAt = DateTime.UtcNow, UpdatedBy = (Guid?)null });
    }
}