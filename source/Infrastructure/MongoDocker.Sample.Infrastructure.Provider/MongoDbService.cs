using MongoDB.Driver;
using MongoDocker.Sample.Domain.Contract.DTO;
using MongoDocker.Sample.Domain.Contract.Exception;
using MongoDocker.Sample.Domain.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MongoDocker.Sample.Infrastructure.Provider
{
    public class MongoDbService : IMongoDbService
    {
        private readonly MongoClient mongoClient;
        private readonly string mongoDatabaseName;
        private readonly string collectionName;

        private const int InsertMaxAttempts = 3;

        /// <summary>
        /// Receives MongoDb configuration values through dependency injection
        /// </summary>
        /// <param name="mongoDbConfigurationValues"></param>
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
            if (key == Guid.Empty)
            {
                throw new MongoDbCustomException(MongoDbCustomError.KeyNotInformed);
            }

            var collection = GetMongoCollection();

            try
            {
                var deletedObject = await collection.FindOneAndDeleteAsync(x => x.Key == key);

                if (deletedObject == null)
                {
                    throw new MongoDbCustomException(MongoDbCustomError.RegisterNotFound);
                }
            }
            catch (TimeoutException ex)
            {
                throw new MongoDbCustomException(MongoDbCustomError.TimeOutServer(GetMongoServerAddressFromTimeoutException(ex)));
            }
        }

        async Task<Guid> IMongoDbService.InsertValueAsync(string value)
        {
            var collection = GetMongoCollection();
            var register = await InsertValueAsync(value, collection);

            return register.Key;
        }

        async Task IMongoDbService.UpdateValueAsync(Guid key, string newValue)
        {
            if (key == Guid.Empty)
            {
                throw new MongoDbCustomException(MongoDbCustomError.KeyNotInformed);
            }

            var collection = GetMongoCollection();

            var filter = Builders<MongoDbRegister>.Filter.Eq(m => m.Key, key);
            var update = Builders<MongoDbRegister>.Update.Set(m => m.Value, newValue);

            try
            {
                var updateResult = await collection.UpdateOneAsync(filter, update);

                if (updateResult.IsModifiedCountAvailable && updateResult.ModifiedCount == 0)
                {
                    throw new MongoDbCustomException(MongoDbCustomError.RegisterNotFound);
                }
            }
            catch (TimeoutException ex)
            {
                throw new MongoDbCustomException(MongoDbCustomError.TimeOutServer(GetMongoServerAddressFromTimeoutException(ex)));
            }
        }

        async Task<MongoDbRegister> IMongoDbService.GetValueAsync(Guid key)
        {
            if (key == Guid.Empty)
            {
                throw new MongoDbCustomException(MongoDbCustomError.KeyNotInformed);
            }

            var collection = GetMongoCollection();

            try
            {
                var filter = Builders<MongoDbRegister>.Filter.Eq(m => m.Key, key);
                var result = await collection.FindAsync(filter);

                if (!result.Any())
                {
                    throw new MongoDbCustomException(MongoDbCustomError.RegisterNotFound);
                }

                return result.FirstOrDefault();
            }
            catch (TimeoutException ex)
            {
                throw new MongoDbCustomException(MongoDbCustomError.TimeOutServer(GetMongoServerAddressFromTimeoutException(ex)));
            }
        }

        async Task<IEnumerable<MongoDbRegister>> IMongoDbService.GetValuesAsync()
        {
            var collection = GetMongoCollection();

            try
            {
                return await collection.AsQueryable().ToListAsync();
            }
            catch (TimeoutException ex)
            {
                throw new MongoDbCustomException(MongoDbCustomError.TimeOutServer(GetMongoServerAddressFromTimeoutException(ex)));
            }
        }

        private IMongoCollection<MongoDbRegister> GetMongoCollection()
        {
            var database = mongoClient.GetDatabase(mongoDatabaseName);

            try
            {
                return database.GetCollection<MongoDbRegister>(collectionName);
            }
            catch (TimeoutException ex)
            {
                throw new MongoDbCustomException(MongoDbCustomError.TimeOutServer(GetMongoServerAddressFromTimeoutException(ex)));
            }
        }

        private async Task<MongoDbRegister> InsertValueAsync(string value, IMongoCollection<MongoDbRegister> collection)
        {
            var register = new MongoDbRegister(value);
            var attempt = 0;

            while (true)
            {
                try
                {
                    await collection.InsertOneAsync(register);

                    return register;
                }
                catch (MongoException ex)
                {
                    var duplicateKeyError = ex.Message.Contains("duplicate key error collection");

                    if (duplicateKeyError && attempt < InsertMaxAttempts)
                    {
                        register.Key = Guid.NewGuid();
                        attempt++;
                    }
                    else if (duplicateKeyError)
                    {
                        throw new MongoDbCustomException(MongoDbCustomError.UnavailableKey);
                    }
                    else
                    {
                        throw ex;
                    }
                }
                catch (TimeoutException ex)
                {
                    throw new MongoDbCustomException(MongoDbCustomError.TimeOutServer(GetMongoServerAddressFromTimeoutException(ex)));
                }
            }
        }

        private string GetMongoServerAddressFromTimeoutException(TimeoutException ex)
        {
            var reg = new Regex(@"((?<![\w\d])localhost(?![\w\d])(\:(\d+)))");
            
            var regMatches = reg.Matches(ex.Message)?
                .Cast<Match>()
                .Where(m => m.Success)?
                .Select(m => m.Value)
                .Distinct();

            return regMatches?.FirstOrDefault() ?? "<unspecified>";
        }
    }
}