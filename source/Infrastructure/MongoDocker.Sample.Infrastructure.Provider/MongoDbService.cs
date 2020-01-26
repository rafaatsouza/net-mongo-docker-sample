using MongoDB.Driver;
using MongoDocker.Sample.Domain.Contract.DTO;
using MongoDocker.Sample.Domain.Contract.Exception;
using MongoDocker.Sample.Domain.Service.Interfaces;
using System;
using System.Threading.Tasks;

namespace MongoDocker.Sample.Infrastructure.Provider
{
    public class MongoDbService : IMongoDbService
    {
        private readonly MongoClient mongoClient;
        private readonly string mongoDatabaseName;
        private readonly string collectionName;

        public MongoDbService(MongoDbConfigurationValues mongoDbConfigurationValues)
        {
            if (mongoDbConfigurationValues == null)
            {
                throw new ArgumentNullException(nameof(mongoDbConfigurationValues));
            }

            if (string.IsNullOrEmpty(mongoDbConfigurationValues.Server))
            {
                throw new ArgumentException($"MongoDB server address is null or empty");
            }

            if (string.IsNullOrEmpty(mongoDbConfigurationValues.Database))
            {
                throw new ArgumentException($"MongoDB database name is null or empty");
            }

            if (string.IsNullOrEmpty(mongoDbConfigurationValues.Collection))
            {
                throw new ArgumentException($"MongoDB collection name is null or empty");
            }

            mongoDatabaseName = mongoDbConfigurationValues.Database;
            collectionName = mongoDbConfigurationValues.Collection;

            mongoClient = new MongoClient(mongoDbConfigurationValues.Server);

            MongoDefaults.GuidRepresentation = MongoDB.Bson.GuidRepresentation.Standard;
        }

        async Task IMongoDbService.DeleteValueAsync(Guid key)
        {
            var database = mongoClient.GetDatabase(mongoDatabaseName);
            var collection = database.GetCollection<MongoDbRegister>(collectionName);

            await collection.FindOneAndDeleteAsync(x => x.Key == key);
        }

        async Task<Guid> IMongoDbService.InsertValueAsync(string value)
        {
            var database = mongoClient.GetDatabase(mongoDatabaseName);
            var collection = database.GetCollection<MongoDbRegister>(collectionName);
            var register = new MongoDbRegister(value);

            await collection.InsertOneAsync(register);

            return register.Key;
        }

        async Task IMongoDbService.UpdateValueAsync(Guid key, string newValue)
        {
            var database = mongoClient.GetDatabase(mongoDatabaseName);
            var collection = database.GetCollection<MongoDbRegister>(collectionName);

            var filter = Builders<MongoDbRegister>.Filter.Eq(m => m.Key, key);
            var update = Builders<MongoDbRegister>.Update.Set(m => m.Value, newValue);

            var updateResult = await collection.UpdateOneAsync(filter, update);

            if (updateResult.IsModifiedCountAvailable && updateResult.ModifiedCount == 0)
            {
                throw new MongoDbCustomException(MongoDbCustomError.RegisterNotFound);
            }
        }

        async Task<MongoDbRegister> IMongoDbService.GetValueAsync(Guid key)
        {
            var database = mongoClient.GetDatabase(mongoDatabaseName);
            var collection = database.GetCollection<MongoDbRegister>(collectionName);

            return (await collection.FindAsync(x => x.Key == key)).FirstOrDefault();
        }
    }
}