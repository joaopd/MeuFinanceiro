using Domain.Entities;

namespace Domain.InterfaceRepository.BaseRepository;

public interface ICardRepository
{
    Task<Card?> GetByIdAsync(Guid id);
    Task<IEnumerable<Card>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Card>> GetFamilyCardsAsync(Guid parentUserId);
    Task AddAsync(Card card);
    Task UpdateAsync(Card card);
}