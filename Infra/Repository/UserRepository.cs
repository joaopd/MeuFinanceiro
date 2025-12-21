using Dapper;
using Domain.Entities;
using Domain.InterfaceRepository;
using Infra.Database;
using Infra.Queries;

namespace Infra.Repository;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Guid> CreateAsync(User user)
    {
        using var connection = _connectionFactory.CreateConnection();

        var id = await connection.QuerySingleAsync<Guid>(
            UserQueries.Insert,
            new
            {
                user.Id,
                user.Name,
                user.ParentUserId,
                user.CreatedAt,
                user.UpdatedAt,
                user.IsDeleted
            });

        return id;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleOrDefaultAsync<User>(
            UserQueries.GetById,
            new { Id = id });
    }

    public async Task<IEnumerable<User>> GetDependentsAsync(Guid parentUserId)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<User>(
            UserQueries.GetDependents,
            new { ParentUserId = parentUserId });
    }
}