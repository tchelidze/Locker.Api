using Locker.Api.Features.LockManagement.Models;
using Locker.Api.Features.Shared;
using Locker.Api.Web.Filters;
using Locker.Domain.Features.Auth.Entities;
using Locker.Domain.Features.LockManagement.Locking;
using Locker.Domain.Features.LockManagement.Repositories;
using Locker.Domain.Features.Shared.ExecutionResult;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Locker.Api.Features.LockManagement
{
    [ValidateModelStateFilter]
    public class LockController : Controller
    {
        private readonly ILockRepository _lockRepository;
        private readonly IMediator _mediator;

        public LockController(ILockRepository lockRepository, IMediator mediator)
        {
            _lockRepository = lockRepository;
            _mediator = mediator;
        }

        [HttpPatch("api/locks/{lockId:objectId}/locking"), CrudApiFilter("Locks@{lockId}", ResourceAccessRight.Update)]
        public async Task<IActionResult> ChangeLockState(string lockId, [BindRequired, FromBody] ChangeLockStateApiInput input)
        {
            var result = await _mediator.Send(new ChangeLockStateCommand
            {
                UserId = User.Claims.First(it => it.Type == ClaimTypes.NameIdentifier).Value,
                LockId = lockId,
                State = input.State
            }).ConfigureAwait(false);

            return new ExecutionResult<LockReadModel>
            {
                Message = result.Message,
                ResultType = result.ResultType,
                Value = result.Value == null ? null : new LockReadModel(result.Value)
            }.ToActionResult();
        }

        [HttpGet("api/locks"), CrudApiFilter("Locks", ResourceAccessRight.Read)]
        public async Task<IActionResult> ReadLocks()
        {
            var locks = await _lockRepository.GetAll().ConfigureAwait(false);

            return new ExecutionResult<IEnumerable<LockReadModel>>
            {
                ResultType = ExecutionResultType.Ok,
                Value = locks.Select(@lock => new LockReadModel(@lock))
            }.ToActionResult();
        }

        [HttpGet("api/locks/{lockId:objectId}"), CrudApiFilter("Locks@{lockId}", ResourceAccessRight.Read)]
        public async Task<IActionResult> GetLock(string lockId)
        {
            var @lock = await _lockRepository.Get(lockId).ConfigureAwait(false);

            if (@lock == null)
            {
                return new ExecutionResult
                {
                    ResultType = ExecutionResultType.NotFound,
                    Message = "Lock not found"
                }.ToActionResult();
            }

            return new ExecutionResult<LockReadModel>
            {
                ResultType = ExecutionResultType.Ok,
                Value = new LockReadModel(@lock)
            }.ToActionResult();
        }

        [HttpGet("api/locks/{lockId:objectId}/locking"), CrudApiFilter("Locks@{lockId}", ResourceAccessRight.Read)]
        public async Task<IActionResult> GetLockActivityHistory(string lockId)
        {
            var lockingHistory = await _lockRepository.GetLockingHistory(lockId).ConfigureAwait(false);

            if (lockingHistory == null)
            {
                return new ExecutionResult
                {
                    ResultType = ExecutionResultType.NotFound,
                    Message = "Lock not found"
                }.ToActionResult();
            }

            return new ExecutionResult<IEnumerable<LockingHistoryItemReadModel>>
            {
                ResultType = ExecutionResultType.Ok,
                Value = lockingHistory.Select(it => new LockingHistoryItemReadModel(it))
            }.ToActionResult();
        }
    }
}