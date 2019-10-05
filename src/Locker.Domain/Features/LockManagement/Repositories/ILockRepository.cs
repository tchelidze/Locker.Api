using System.Collections.Generic;
using System.Threading.Tasks;
using Locker.Domain.Features.LockManagement.Entities;

namespace Locker.Domain.Features.LockManagement.Repositories
{
    public interface ILockRepository
    {
        Task<Lock> TryAdd(Lock @lock);

        Task<Lock> TryAddLocking(string lockId, LockingHistoryItem activityHistoryItem);

        Task<LockingHistoryItem[]> GetLockingHistory(string lockId);

        Task<Lock> Get(string lockId);

        Task<List<Lock>> GetAll();
    }
}