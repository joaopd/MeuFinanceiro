using Domain.Entities;

namespace Domain.InterfaceRepository;

public interface IUserRepository
{
    Task<Guid> CreateAsync(User user);
    Task<User?> GetByIdAsync(Guid id);
    Task<IEnumerable<User>> GetDependentsAsync(Guid parentUserId);
}
