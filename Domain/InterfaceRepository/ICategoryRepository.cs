using Domain.Entities;

namespace Domain.InterfaceRepository;

public interface ICategoryRepository
{
    Task InsertAsync(Category category);

    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(Guid id);
    Task<Category?> GetByNameAsync(string name);
}