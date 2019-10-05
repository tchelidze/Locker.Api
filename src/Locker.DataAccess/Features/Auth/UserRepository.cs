using System;
using System.Linq;
using System.Threading.Tasks;
using Locker.DataAccess.Features.Shared;
using Locker.Domain.Features.Auth.Entities;
using Locker.Domain.Features.Auth.Repositories;
using MongoDB.Driver;

namespace Locker.DataAccess.Features.Auth
{
    public class UserRepository : BaseMongoRepository<User>, IUserRepository
    {
        public UserRepository(MongoRepositorySettings settings) : base(settings, "users")
        {
        }

        public override Task ConfigureCollection() =>
            Collection.Indexes
                .CreateOneAsync(new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(it => it.UserName),
                    new CreateIndexOptions
                    {
                        Unique = true
                    }));

        public Task<User> FindByUsername(string username)
            => Collection.Find(Query.Eq(it => it.UserName, username)).SingleOrDefaultAsync();

        public Task AddRefreshToken(string userId, string refreshToken) =>
            Collection.FindOneAndUpdateAsync(Query.Eq(it => it.Id, userId), Update.Push(it => it.RefreshTokens,
                new UserRefreshToken
                {
                    Token = refreshToken,
                    CreatedOn = DateTime.UtcNow
                }));

        public async Task<bool> TryUseRefreshToken(string userId, string refreshToken)
        {
            var updateResult = await Collection.UpdateOneAsync(
                    Query.Eq(it => it.Id, userId) &
                    Query.ElemMatch(it => it.RefreshTokens, token => token.Token == refreshToken),
                    Update.PullFilter(user => user.RefreshTokens, token => token.Token == refreshToken))
                .ConfigureAwait(false);

            return updateResult.ModifiedCount == 1;
        }

        public async Task<User> TryAddUser(User user)
        {
            try
            {
                await Collection.InsertOneAsync(user).ConfigureAwait(false);
                return user;
            }
            catch (MongoWriteException ex) when (ex.Message.Contains("E11000 duplicate key"))
            {
                return null;
            }
        }

        public Task<User> Get(string id) =>
            Collection.Find(Query.Eq(it => it.Id, id)).SingleOrDefaultAsync();

        public Task<User> SetPermissions(string userId, UserPermission[] permissions)
        {
            return Collection.FindOneAndUpdateAsync(Query.Eq(it => it.Id, userId),
                Update.Set(it => it.Permissions, permissions),
                new FindOneAndUpdateOptions<User, User>
                {
                    ReturnDocument = ReturnDocument.After
                });
        }

        public Task<User> RemovePermissions(string userId, string[] resources) =>
            Collection.FindOneAndUpdateAsync(Query.Eq(it => it.Id, userId),
                Update.PullFilter(it => it.Permissions, permission => resources.Contains(permission.Resource)),
                new FindOneAndUpdateOptions<User, User>
                {
                    ReturnDocument = ReturnDocument.After
                });
    }
}