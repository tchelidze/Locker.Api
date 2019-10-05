using System.Linq;
using Locker.Domain.Features.Auth.Entities;

namespace Locker.Api.Features.UserManagement.Models
{
    public class UserReadModel
    {
        public UserReadModel(User user)
        {
            Id = user.Id;
            UserName = user.UserName;

            Permission = user.Permissions.Select(it => new UserPermissionReadModel(it)).ToArray();
        }

        public string Id { get; set; }

        public string UserName { get; set; }

        public UserPermissionReadModel[] Permission { get; }
    }

    public class UserPermissionReadModel
    {
        public UserPermissionReadModel(UserPermission permission)
        {
            Resource = permission.Resource;
            AccessRight = permission.AccessRight;
        }

        public string Resource { get; set; }

        public ResourceAccessRight AccessRight { get; set; }
    }
}
