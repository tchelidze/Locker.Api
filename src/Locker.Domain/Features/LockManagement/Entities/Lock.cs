using System;
using System.Diagnostics;

namespace Locker.Domain.Features.LockManagement.Entities
{
    [DebuggerDisplay("{FriendlyName} {Id}")]
    public class Lock
    {
        public string Id { get; set; }

        public string FriendlyName { get; set; }

        public LockingHistoryItem[] LockingHistory { get; set; } = new LockingHistoryItem[0];
    }

    [DebuggerDisplay("{State} {OccuredOn} {UserId}")]
    public class LockingHistoryItem
    {
        public LockingState State { get; set; }

        public DateTime OccuredOn { get; set; }

        public string UserId { get; set; }
    }

    public enum LockingState
    {
        Unlocked = 1
    }
}