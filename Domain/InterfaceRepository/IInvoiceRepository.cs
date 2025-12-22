using Domain.Entities;

namespace Domain.InterfaceRepository;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByCardAndDateAsync(Guid cardId, DateTime referenceDate);
    Task<IEnumerable<Invoice>> GetByUserIdAsync(Guid userId);
    Task<Invoice?> GetByIdAsync(Guid id);
    Task<bool> InsertAsync(Invoice invoice);
    Task<bool> UpdateAsync(Invoice invoice);
    Task<bool> PayInvoiceFullAsync(Guid invoiceId, Guid userId);
}