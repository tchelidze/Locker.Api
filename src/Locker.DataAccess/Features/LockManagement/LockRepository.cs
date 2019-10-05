using System.Collections.Generic;
using System.Threading.Tasks;
using Locker.DataAccess.Features.Shared;
using Locker.Domain.Features.LockManagement.Entities;
using Locker.Domain.Features.LockManagement.Repositories;
using MongoDB.Driver;

namespace Locker.DataAccess.Features.LockManagement
{
    public class LockRepository : BaseMongoRepository<Lock>, ILockRepository
    {
        public LockRepository(MongoRepositorySettings settings)
            : base(settings, "locks")
        { }

        public override Task ConfigureCollection() =>
            Collection.Indexes
                .CreateOneAsync(new CreateIndexModel<Lock>(Builders<Lock>.IndexKeys.Ascending(it => it.FriendlyName),
                    new CreateIndexOptions
                    {
                        Unique = true
                    }));

        public async Task<Lock> TryAdd(Lock @lock)
        {
            try
            {
                await Collection.InsertOneAsync(@lock).ConfigureAwait(false);
                return @lock;
            }
            catch (MongoWriteException ex) when (ex.Message.Contains("E11000 duplicate key"))
            {
                return null;
            }
        }

        public Task<Lock> TryAddLocking(string lockId, LockingHistoryItem activityHistoryItem) =>
            Collection.FindOneAndUpdateAsync(Query.Eq(it => it.Id, lockId),
                Update.Push(it => it.LockingHistory, activityHistoryItem),
                new FindOneAndUpdateOptions<Lock, Lock>
                {
                    IsUpsert = false,
                    ReturnDocument = ReturnDocument.After
                });

        public async Task<LockingHistoryItem[]> GetLockingHistory(string lockId)
        {
            var door = await Collection.Find(Query.Eq(it => it.Id, lockId)).SingleOrDefaultAsync().ConfigureAwait(false);
            return door?.LockingHistory;
        }

        public Task<Lock> Get(string lockId)
            => Collection.Find(Query.Eq(it => it.Id, lockId)).SingleOrDefaultAsync();

        public async Task<List<Lock>> GetAll()
            => await Collection.Find(Query.Empty).ToListAsync().ConfigureAwait(false);
    }
}