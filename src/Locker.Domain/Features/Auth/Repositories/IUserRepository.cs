using System.Threading.Tasks;
using Locker.Domain.Features.Auth.Entities;

namespace Locker.Domain.Features.Auth.Repositories
{
    public interface IUserRepository
    {
        Task<User> FindByUsername(string username);

        Task AddRefreshToken(string userId, string refreshToken);

        Task<bool> TryUseRefreshToken(string userId, string refreshToken);

        Task<User> TryAddUser(User user);

        Task<User> Get(string id);

        Task<User> SetPermissions(string userId, UserPermission[] permissions);

        Task<User> RemovePermissions(string userId, string[] resources);
    }
}