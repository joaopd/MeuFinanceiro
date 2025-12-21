using Domain.Entities;
using Domain.InterfaceRepository.BaseRepository;

namespace Domain.InterfaceRepository;

public interface IFixedExpenseRepository : IRepository<FixedExpense>
{
    Task<IEnumerable<FixedExpense>> GetActiveAsync(
        Guid userId,
        DateTime referenceDate
    );
}
