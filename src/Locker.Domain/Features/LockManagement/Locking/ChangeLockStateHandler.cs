using System;
using System.Threading;
using System.Threading.Tasks;
using Locker.Domain.Features.LockManagement.Entities;
using Locker.Domain.Features.LockManagement.Repositories;
using Locker.Domain.Features.Shared.ExecutionResult;
using MediatR;

namespace Locker.Domain.Features.LockManagement.Locking
{
    public class ChangeLockStateHandler : IRequestHandler<ChangeLockStateCommand, ExecutionResult<Lock>>
    {
        private readonly ILockRepository _lockRepository;

        public ChangeLockStateHandler(ILockRepository lockRepository)
        {
            _lockRepository = lockRepository;
        }

        public async Task<ExecutionResult<Lock>> Handle(ChangeLockStateCommand command, CancellationToken cancellationToken)
        {
            var door = await _lockRepository.TryAddLocking(command.LockId, new LockingHistoryItem
            {
                State = LockingState.Unlocked,
                UserId = command.UserId,
                OccuredOn = DateTime.UtcNow
            }).ConfigureAwait(false);

            if (door == null)
            {
                return new ExecutionResult<Lock>
                {
                    ResultType = ExecutionResultType.NotFound,
                    Message = "Door not found"
                };
            }

            return new ExecutionResult<Lock>
            {
                ResultType = ExecutionResultType.Ok,
                Value = door
            };
        }
    }
}
