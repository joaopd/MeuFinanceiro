using Domain.Entities;
using Domain.InterfaceRepository.BaseRepository;

namespace Domain.InterfaceRepository;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByNameAsync(string name);
}