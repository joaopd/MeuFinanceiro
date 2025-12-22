using Domain.Entities;
using Domain.InterfaceRepository.BaseRepository;

namespace Domain.InterfaceRepository;

public interface IInvoiceRepository : IRepository<Invoice>
{
    Task<Invoice?> GetByCardAndDateAsync(
        Guid cardId,
        DateTime referenceDate
    );

    Task<IEnumerable<Invoice>> GetByUserIdAsync(
        Guid userId
    );

    Task PayInvoiceFullAsync(
        Guid invoiceId,
        Guid userId
    );
}