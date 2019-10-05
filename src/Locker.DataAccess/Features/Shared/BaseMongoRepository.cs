using System.Threading.Tasks;
using MongoDB.Driver;

namespace Locker.DataAccess.Features.Shared
{
    public interface IConfigureCollection
    {
        Task ConfigureCollection();
    }

    public abstract class BaseMongoRepository<TModel> : IConfigureCollection
    {
        protected MongoClient MongoClient;

        protected IMongoDatabase MongoDatabase;

        protected readonly IMongoCollection<TModel> Collection;

        protected BaseMongoRepository(MongoRepositorySettings settings, string collectionName)
        {
            MongoClient = new MongoClient(settings.ConnectionString);
            MongoDatabase = MongoClient.GetDatabase(settings.DatabaseName);
            Collection = MongoDatabase.GetCollection<TModel>(collectionName);
        }

        protected FilterDefinitionBuilder<TModel> Query = Builders<TModel>.Filter;

        protected UpdateDefinitionBuilder<TModel> Update = Builders<TModel>.Update;

        public virtual Task ConfigureCollection() => Task.CompletedTask;

    }

    public class MongoRepositorySettings
    {
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }
    }
}
