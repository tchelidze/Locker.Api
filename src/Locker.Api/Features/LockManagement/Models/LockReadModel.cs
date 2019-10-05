using System;
using System.Linq;
using Locker.Domain.Features.LockManagement.Entities;

namespace Locker.Api.Features.LockManagement.Models
{
    public class LockReadModel
    {
        public LockReadModel(Lock @lock)
        {
            Id = @lock.Id;
            FriendlyName = @lock.FriendlyName;
            ActivityHistory = @lock.LockingHistory?.Select(it => new LockingHistoryItemReadModel(it)).ToArray();
        }

        public string Id { get; set; }

        public string FriendlyName { get; set; }

        public LockingHistoryItemReadModel[] ActivityHistory { get; set; }
    }

    public class LockingHistoryItemReadModel
    {
        public LockingHistoryItemReadModel(LockingHistoryItem historyItem)
        {
            State = historyItem.State.ToString();
            OccuredOn = historyItem.OccuredOn;
            UserId = historyItem.UserId;
        }

        public string State { get; set; }

        public DateTime OccuredOn { get; set; }

        public string UserId { get; set; }
    }
}
