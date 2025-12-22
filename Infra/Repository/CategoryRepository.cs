using Dapper;
using Domain.Entities;
using Domain.InterfaceRepository;
using Infra.Database;
using Infra.Queries;

namespace Infra.Repository;

public class CategoryRepository : ICategoryRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CategoryRepository(IDbConnectionFactory connectionFactory)
        => _connectionFactory = connectionFactory;

    public async Task<Guid> InsertAsync(Category category)
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QuerySingleAsync<Guid>(
            CategoryQueries.Insert,
            category);
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryAsync<Category>(
            CategoryQueries.GetAll);
    }

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Category>(
            CategoryQueries.GetById,
            new { Id = id });
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Category>(
            CategoryQueries.GetByName,
            new { Name = name });
    }

    public async Task UpdateAsync(Category entity)
    {
        using var conn = _connectionFactory.CreateConnection();
        await conn.ExecuteAsync(
            CategoryQueries.Update,
            new
            {
                entity.Id,
                entity.Name,
                entity.UpdatedAt,
                entity.UpdatedBy
            });
    }

    public async Task DeleteAsync(Guid id)
    {
        using var conn = _connectionFactory.CreateConnection();
        await conn.ExecuteAsync(
            CategoryQueries.SoftDelete,
            new { Id = id, UpdatedAt = DateTime.UtcNow });
    }
}
