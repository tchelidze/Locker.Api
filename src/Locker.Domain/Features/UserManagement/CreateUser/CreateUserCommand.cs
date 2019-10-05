using Locker.Domain.Features.Auth.Entities;
using Locker.Domain.Features.Shared.ExecutionResult;
using MediatR;

namespace Locker.Domain.Features.UserManagement.CreateUser
{
    public class CreateUserCommand : IRequest<ExecutionResult<User>>
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public UserPermission[] Permissions { get; set; }

    }
}
