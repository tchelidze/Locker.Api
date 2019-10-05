using System.Threading;
using System.Threading.Tasks;
using Locker.Domain.Features.Auth.Entities;
using Locker.Domain.Features.Auth.Repositories;
using Locker.Domain.Features.Shared.ExecutionResult;
using MediatR;

namespace Locker.Domain.Features.UserManagement.RemovePermission
{
    public class RemoveUserPermissionHandler : IRequestHandler<RemoveUserPermissionCommand, ExecutionResult<User>>
    {
        private readonly IUserRepository _userRepository;

        public RemoveUserPermissionHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ExecutionResult<User>> Handle(RemoveUserPermissionCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.RemovePermissions(command.UserId, command.Resources).ConfigureAwait(false);

            if (user == null)
            {
                return new ExecutionResult<User>
                {
                    ResultType = ExecutionResultType.NotFound,
                    Message = "User cannot be found."
                };
            }

            return new ExecutionResult<User>
            {
                ResultType = ExecutionResultType.Ok,
                Value = user
            };
        }
    }
}
