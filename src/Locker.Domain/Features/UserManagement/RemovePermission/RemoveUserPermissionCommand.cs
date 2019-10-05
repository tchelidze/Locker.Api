using Locker.Domain.Features.Auth.Entities;
using Locker.Domain.Features.Shared.ExecutionResult;
using MediatR;

namespace Locker.Domain.Features.UserManagement.RemovePermission
{
    public class RemoveUserPermissionCommand : IRequest<ExecutionResult<User>>
    {
        public string UserId { get; set; }

        public string[] Resources { get; set; }
    }
}
