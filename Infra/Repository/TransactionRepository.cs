using Dapper;
using Domain.Entities;
using Domain.Enums;
using Domain.InterfaceRepository;
using Domain.Records;
using Infra.Database;
using Infra.Queries;

namespace Infra.Repository;

public class TransactionRepository : ITransactionRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TransactionRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Transaction?> GetByIdAsync(Guid id)
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Transaction>(
            TransactionQueries.GetById,
            new { Id = id });
    }

    public async Task<IEnumerable<Transaction>> GetAllAsync()
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryAsync<Transaction>(
            TransactionQueries.GetAll);
    }

    public async Task<IEnumerable<Transaction>> GetByUserAndPeriodAsync(
        Guid userId,
        DateTime startDate,
        DateTime endDate)
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryAsync<Transaction>(
            TransactionQueries.GetByUserAndPeriod,
            new { UserId = userId, StartDate = startDate, EndDate = endDate });
    }

    public async Task<decimal> GetBalanceByPeriodAsync(
        Guid userId,
        DateTime startDate,
        DateTime endDate)
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.ExecuteScalarAsync<decimal>(
            TransactionQueries.GetBalanceByPeriod,
            new { UserId = userId, StartDate = startDate, EndDate = endDate });
    }

    public async Task<bool> InsertBulkAsync(IEnumerable<Transaction> entities)
    {
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();

        using var transaction = conn.BeginTransaction();

        try
        {
            await conn.ExecuteAsync(
                TransactionQueries.Insert,
                entities,
                transaction);

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }


    public async Task<Guid> InsertAsync(Transaction entity)
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QuerySingleAsync<Guid>(
            TransactionQueries.Insert,
            entity);
    }

    public async Task UpdateAsync(Transaction entity)
    {
        using var conn = _connectionFactory.CreateConnection();
        await conn.ExecuteAsync(
            TransactionQueries.Update,
            new 
            { 
                entity.Id, 
                entity.Amount, 
                entity.TransactionDate, 
                entity.UpdatedAt,
                entity.UpdatedBy
            });
    }

    public async Task DeleteAsync(Guid id)
    {
        using var conn = _connectionFactory.CreateConnection();
        await conn.ExecuteAsync(
            TransactionQueries.SoftDelete,
            new { Id = id, UpdatedAt = DateTime.UtcNow });
    }
    
    public async Task<IEnumerable<TransactionPagedRow>> GetByUserAndPeriodPagedAsync(
        Guid userId,
        DateTime startDate,
        DateTime endDate,
        TransactionType? transactionType,
        int currentPage,
        int rowsPerPage,
        string? orderBy,
        bool orderAsc)
    {
        using var conn = _connectionFactory.CreateConnection();

        return await conn.QueryAsync<TransactionPagedRow>(
            TransactionQueries.GetByUserAndPeriodPagedWithMeta,
            new
            {
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate,
                TransactionType = transactionType,
                CurrentPage = currentPage,
                RowsPerPage = rowsPerPage,
                OrderBy = orderBy,
                OrderAsc = orderAsc
            });
    }

}
