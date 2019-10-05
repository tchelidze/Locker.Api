using System.Threading;
using System.Threading.Tasks;
using Locker.Domain.Features.Auth.Entities;
using Locker.Domain.Features.Auth.Repositories;
using Locker.Domain.Features.Shared.ExecutionResult;
using MediatR;

namespace Locker.Domain.Features.UserManagement.CreateUser
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, ExecutionResult<User>>
    {
        private readonly IUserRepository _userRepository;

        public CreateUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ExecutionResult<User>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
        {
            var user = new User
            {
                UserName = command.UserName,
                Permissions = command.Permissions ?? new UserPermission[0]
            };

            user.SetPassword(command.Password);

            user = await _userRepository.TryAddUser(user).ConfigureAwait(false);

            if (user == null)
            {
                return new ExecutionResult<User>
                {
                    ResultType = ExecutionResultType.ValidationError,
                    Message = "User with same username already exists"
                };
            }

            return new ExecutionResult<User>
            {
                Value = user,
                ResultType = ExecutionResultType.Ok
            };
        }
    }
}
