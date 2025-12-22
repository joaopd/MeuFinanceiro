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



    public async Task<Invoice?> GetByIdAsync(Guid id)
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Invoice>(
            InvoiceQueries.GetById,
            new { Id = id });
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync()
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryAsync<Invoice>(InvoiceQueries.GetAll);
    }

    public async Task<Guid> InsertAsync(Invoice invoice)
    {
        using var conn = _connectionFactory.CreateConnection();

        await conn.ExecuteAsync(InvoiceQueries.Insert, invoice);
        return invoice.Id;
    }

    public async Task UpdateAsync(Invoice invoice)
    {
        using var conn = _connectionFactory.CreateConnection();
        await conn.ExecuteAsync(InvoiceQueries.Update, invoice);
    }

    public async Task DeleteAsync(Guid id)
    {
        using var conn = _connectionFactory.CreateConnection();
        await conn.ExecuteAsync(InvoiceQueries.Delete, new { Id = id });
    }

    public async Task<Invoice?> GetByCardAndDateAsync(Guid cardId, DateTime referenceDate)
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Invoice>(
            InvoiceQueries.GetByCardAndDate,
            new
            {
                CardId = cardId,
                Month = referenceDate.Month,
                Year = referenceDate.Year
            });
    }

    public async Task<IEnumerable<Invoice>> GetByUserIdAsync(Guid userId)
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryAsync<Invoice>(
            InvoiceQueries.GetByUserId,
            new { UserId = userId });
    }

    public async Task PayInvoiceFullAsync(Guid invoiceId, Guid userId)
    {
        using var conn = _connectionFactory.CreateConnection();
        conn.Open();

        using var transaction = conn.BeginTransaction();

        try
        {
            // 1. Marca fatura como paga
            await conn.ExecuteAsync(
                InvoiceQueries.PayInvoice,
                new { InvoiceId = invoiceId, UserId = userId },
                transaction);

            // 2. Marca transações vinculadas como pagas
            await conn.ExecuteAsync(
                InvoiceQueries.PayRelatedTransactions,
                new { InvoiceId = invoiceId, UserId = userId },
                transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
