using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Locker.Domain.Features.Auth.Entities;
using Locker.Domain.Features.Auth.Repositories;
using Locker.Domain.Features.Shared.ExecutionResult;
using MediatR;

namespace Locker.Domain.Features.UserManagement.AddPermission
{
    public class AddUserPermissionHandler : IRequestHandler<AddUserPermissionCommand, ExecutionResult<User>>
    {
        private readonly IUserRepository _userRepository;

        public AddUserPermissionHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ExecutionResult<User>> Handle(AddUserPermissionCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.Get(command.UserId).ConfigureAwait(false);

            if (user == null)
            {
                return new ExecutionResult<User>
                {
                    ResultType = ExecutionResultType.NotFound,
                    Message = "User cannot be found."
                };
            }

            var permissions = user.Permissions.Where(permission =>
                    !command.Permissions.Select(nerPermission => nerPermission.Resource).Contains(permission.Resource))
                .Concat(command.Permissions).ToArray();

            var updatedUser = await _userRepository.SetPermissions(command.UserId, permissions).ConfigureAwait(false);

            return new ExecutionResult<User>
            {
                ResultType = ExecutionResultType.Ok,
                Value = updatedUser
            };
        }
    }
}
