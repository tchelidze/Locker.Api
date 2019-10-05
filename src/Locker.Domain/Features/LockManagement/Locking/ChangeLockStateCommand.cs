using Locker.Domain.Features.LockManagement.Entities;
using Locker.Domain.Features.Shared.ExecutionResult;
using MediatR;

namespace Locker.Domain.Features.LockManagement.Locking
{
    public class ChangeLockStateCommand : IRequest<ExecutionResult<Lock>>
    {
        public string UserId { get; set; }

        public string LockId { get; set; }

        public LockingState State { get; set; }
    }
}
