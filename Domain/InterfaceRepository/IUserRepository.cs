using Domain.Entities;
using Domain.InterfaceRepository.BaseRepository;

namespace Domain.InterfaceRepository;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);

    Task<IEnumerable<User>> GetDependentsAsync(Guid parentUserId);

    Task<IEnumerable<User>> GetByParentIdAsync(Guid parentUserId);
}