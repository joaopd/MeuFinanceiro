using Dapper;
using Domain.Entities;
using Domain.InterfaceRepository;
using Infra.Database;
using Infra.Queries;

namespace Infra.Repository;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public InvoiceRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<Invoice?> GetByCardAndDateAsync(Guid cardId, DateTime referenceDate)
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Invoice>(
            InvoiceQueries.GetByCardAndDate, 
            new { CardId = cardId, Month = referenceDate.Month, Year = referenceDate.Year });
    }

    public async Task<IEnumerable<Invoice>> GetByUserIdAsync(Guid userId)
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryAsync<Invoice>(InvoiceQueries.GetByUserId, new { UserId = userId });
    }

    public async Task<bool> InsertAsync(Invoice invoice)
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.ExecuteAsync(InvoiceQueries.Insert, invoice) > 0;
    }

    public async Task<Invoice?> GetByIdAsync(Guid id)
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Invoice>(InvoiceQueries.GetById, new { Id = id });
    }

    public async Task<bool> PayInvoiceFullAsync(Guid invoiceId, Guid userId)
    {
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();
        using var transaction = conn.BeginTransaction();
        try
        {
            // 1. Atualiza a fatura para paga
            await conn.ExecuteAsync(InvoiceQueries.PayInvoice, new { InvoiceId = invoiceId, UserId = userId }, transaction);
            
            // 2. Atualiza todas as transações vinculadas para pagas
            await conn.ExecuteAsync(InvoiceQueries.PayRelatedTransactions, new { InvoiceId = invoiceId, UserId = userId }, transaction);

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            return false;
        }
    }
}