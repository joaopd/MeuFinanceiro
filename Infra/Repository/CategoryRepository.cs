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
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InsertAsync(Category category)
    {
        using var conn = _connectionFactory.CreateConnection();
        await conn.ExecuteAsync(CategoryQueries.Insert, category);
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QueryAsync<Category>(CategoryQueries.GetAll);
    }

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        using var conn = _connectionFactory.CreateConnection();
        return await conn.QuerySingleOrDefaultAsync<Category>(
            CategoryQueries.GetById,
            new { Id = id });
        
    }
}