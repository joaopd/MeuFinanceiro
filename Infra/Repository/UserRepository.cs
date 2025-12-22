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
        => _connectionFactory = connectionFactory;

    public async Task<Guid> InsertAsync(User user)
    {
        using var conn = _connectionFactory.CreateConnection();

        return await conn.QuerySingleAsync<Guid>(
            UserQueries.Insert,
            new
            {
                user.Id,
                user.Name,
                user.Email,
                user.ParentUserId,
                user.CreatedAt,
                user.UpdatedAt,
                user.IsDeleted
            });
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<User>(
            UserQueries.GetById,
            new { Id = id });
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryAsync<User>(
            UserQueries.GetAll);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<User>(
            UserQueries.GetByEmail,
            new { Email = email });
    }

    public async Task<IEnumerable<User>> GetDependentsAsync(Guid parentUserId)
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryAsync<User>(
            UserQueries.GetDependents,
            new { ParentUserId = parentUserId });
    }

    public async Task<IEnumerable<User>> GetByParentIdAsync(Guid parentUserId)
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryAsync<User>(
            UserQueries.GetByParentId,
            new { ParentUserId = parentUserId });
    }

    public async Task UpdateAsync(User user)
    {
        using var conn = _connectionFactory.CreateConnection();
        await conn.ExecuteAsync(
            UserQueries.Update,
            new
            {
                user.Id,
                user.Name,
                user.Email,
                user.UpdatedAt,
                user.UpdatedBy
            });
    }

    public async Task DeleteAsync(Guid id)
    {
        using var conn = _connectionFactory.CreateConnection();
        await conn.ExecuteAsync(
            UserQueries.SoftDelete,
            new { Id = id, UpdatedAt = DateTime.UtcNow });
    }
}
