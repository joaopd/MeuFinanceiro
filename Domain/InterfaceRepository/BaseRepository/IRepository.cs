namespace Domain.InterfaceRepository.BaseRepository;

public interface IRepository<T>
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<Guid> InsertAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}
