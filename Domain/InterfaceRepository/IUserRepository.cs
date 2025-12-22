using Domain.Entities;

namespace Domain.InterfaceRepository;

public interface IUserRepository
{
    Task<Guid> CreateAsync(User user);
    Task<User?> GetByIdAsync(Guid id);
    Task<IEnumerable<User>> GetDependentsAsync(Guid parentUserId);
    Task<IEnumerable<User>> GetByParentIdAsync(Guid parentUserId);
    Task<User?> GetByEmailAsync(string email);
    
}
