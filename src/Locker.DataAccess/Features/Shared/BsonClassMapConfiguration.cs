using Locker.Domain.Features.Auth.Entities;
using Locker.Domain.Features.LockManagement.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace Locker.DataAccess.Features.Shared
{
    public class BsonClassMapConfiguration
    {
        /// <summary>
        /// Class map should be configure till any connection will be made to db.
        /// </summary>
        public static void ConfigureDbClassMap()
        {
            ConfigureLockBsonClassMap();
            ConfigureUserBsonClassMap();
        }

        private static void ConfigureLockBsonClassMap()
        {
            BsonClassMap.RegisterClassMap<Lock>(map =>
            {
                map.AutoMap();
                map
                    .MapIdProperty(c => c.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId));
            });
        }

        private static void ConfigureUserBsonClassMap()
        {
            BsonClassMap.RegisterClassMap<User>(map =>
            {
                map.AutoMap();
                map
                    .MapIdProperty(it => it.Id)
                    .SetIdGenerator(StringObjectIdGenerator.Instance)
                    .SetSerializer(new StringSerializer(BsonType.ObjectId));
            });
        }
    }
}