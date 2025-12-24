using Dapper;
using Domain.Entities;
using Domain.InterfaceRepository.BaseRepository;
using Infra.Database;
using Infra.Queries;

namespace Infra.Repository;

public class CardRepository(IDbConnectionFactory connectionFactory) : ICardRepository
{
    public async Task AddAsync(Card card)
    {
        using var conn = connectionFactory.CreateConnection();
        await conn.ExecuteAsync(CardQueries.Insert, card);
    }

    public async Task UpdateAsync(Card card)
    {
        using var conn = connectionFactory.CreateConnection();
        await conn.ExecuteAsync(CardQueries.Update, card);
    }

    public async Task<Card?> GetByIdAsync(Guid id)
    {
        using var conn = connectionFactory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Card>(CardQueries.GetById, new { Id = id });
    }

    public async Task<IEnumerable<Card>> GetByUserIdAsync(Guid userId)
    {
        using var conn = connectionFactory.CreateConnection();
        return await conn.QueryAsync<Card>(CardQueries.GetByUserId, new { UserId = userId });
    }

    public async Task<IEnumerable<Card>> GetFamilyCardsAsync(Guid parentUserId)
    {
        using var conn = connectionFactory.CreateConnection();
        return await conn.QueryAsync<Card>(CardQueries.GetFamilyCards, new { UserId = parentUserId });
    }
}