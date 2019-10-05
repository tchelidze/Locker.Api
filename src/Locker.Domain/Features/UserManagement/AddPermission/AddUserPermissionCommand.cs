using Locker.Domain.Features.Auth.Entities;
using Locker.Domain.Features.Shared.ExecutionResult;
using MediatR;

namespace Locker.Domain.Features.UserManagement.AddPermission
{
    public class AddUserPermissionCommand : IRequest<ExecutionResult<User>>
    {
        public string UserId { get; set; }

        public UserPermission[] Permissions { get; set; }
    }
}
