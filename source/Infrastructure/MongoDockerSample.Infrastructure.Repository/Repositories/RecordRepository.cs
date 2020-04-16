using AutoMapper;
using MongoDB.Driver;
using MongoDockerSample.Core.Domain.Models;
using MongoDockerSample.Core.Domain.Repositories;
using MongoDockerSample.Infrastructure.Repository.Dtos;
using MongoDockerSample.Infrastructure.Repository.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MongoDockerSample.Infrastructure.Repository.Repositories
{
    public class RecordRepository : IRecordRepository
    {
        private readonly IMapper mapper;
        private readonly MongoClient mongoClient;
        private readonly string mongoDatabaseName;
        private readonly string collectionName;

        private const int InsertMaxAttempts = 3;

        /// <summary>
        /// Receives MongoDb configuration values through dependency injection
        /// </summary>
        /// <param name="configurationValues"></param>
        public RecordRepository(IMapper mapper, RepositoryConfiguration configurationValues)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            if (configurationValues == null)
            {
                throw new ArgumentNullException(nameof(configurationValues));
            }

            if (string.IsNullOrEmpty(configurationValues.MongoServer))
            {
                throw new ArgumentException($"{nameof(configurationValues.MongoServer)} is null or empty");
            }

            if (string.IsNullOrEmpty(configurationValues.MongoDatabase))
            {
                throw new ArgumentException($"{nameof(configurationValues.MongoDatabase)} is null or empty");
            }

            if (string.IsNullOrEmpty(configurationValues.MongoCollection))
            {
                throw new ArgumentException($"{nameof(configurationValues.MongoCollection)} is null or empty");
            }

            mongoDatabaseName = configurationValues.MongoDatabase;
            collectionName = configurationValues.MongoCollection;

            mongoClient = new MongoClient(configurationValues.MongoServer);
        }

        async Task<Guid> IRecordRepository.InsertRecordAsync(string value)
        {
            var collection = GetMongoCollection();
            var record = await InsertValueAsync(value, collection);

            return record.Key;
        }

        async Task<int> IRecordRepository.UpdateRecordAsync(Guid key, string newValue)
        {
            var collection = GetMongoCollection();

            var filter = Builders<RecordDto>.Filter.Eq(m => m.Key, key);
            var update = Builders<RecordDto>.Update.Set(m => m.Value, newValue);

            try
            {
                var updateResult = await collection.UpdateOneAsync(filter, update);

                return updateResult.IsModifiedCountAvailable ? (int)updateResult.ModifiedCount : 0;
            }
            catch (TimeoutException ex)
            {
                throw new RepositoryCustomException(RepositoryCustomError.TimeOutServer(GetMongoServerAddressFromTimeoutException(ex)));
            }
        }

        async Task<int> IRecordRepository.DeleteRecordAsync(Guid key)
        {
            var collection = GetMongoCollection();

            try
            {
                var deletedRecord = await collection.FindOneAndDeleteAsync(x => x.Key == key);

                return deletedRecord != null ? 1 : 0;
            }
            catch (TimeoutException ex)
            {
                throw new RepositoryCustomException(RepositoryCustomError.TimeOutServer(GetMongoServerAddressFromTimeoutException(ex)));
            }
        }        

        async Task<Record> IRecordRepository.GetRecordAsync(Guid key)
        {
            var collection = GetMongoCollection();

            try
            {
                var filter = Builders<RecordDto>.Filter.Eq(m => m.Key, key);
                var result = (await collection.FindAsync(filter)).ToList();

                if (!result.Any())
                {
                    return null;
                }

                return mapper.Map<Record>(result.FirstOrDefault());
            }
            catch (TimeoutException ex)
            {
                throw new RepositoryCustomException(RepositoryCustomError.TimeOutServer(GetMongoServerAddressFromTimeoutException(ex)));
            }
        }

        async Task<IEnumerable<Record>> IRecordRepository.GetRecordsAsync()
        {
            var collection = GetMongoCollection();

            try
            {
                var records = await collection.AsQueryable().ToListAsync();

                if (records == null || !records.Any())
                {
                    return new List<Record>();
                }

                return mapper.Map<List<Record>>(records);
            }
            catch (TimeoutException ex)
            {
                throw new RepositoryCustomException(RepositoryCustomError.TimeOutServer(GetMongoServerAddressFromTimeoutException(ex)));
            }
        }

        private IMongoCollection<RecordDto> GetMongoCollection()
        {
            var database = mongoClient.GetDatabase(mongoDatabaseName);

            try
            {
                return database.GetCollection<RecordDto>(collectionName);
            }
            catch (TimeoutException ex)
            {
                throw new RepositoryCustomException(RepositoryCustomError.TimeOutServer(GetMongoServerAddressFromTimeoutException(ex)));
            }
        }

        private async Task<RecordDto> InsertValueAsync(string value, IMongoCollection<RecordDto> collection)
        {
            var record = new RecordDto(value);
            var attempt = 0;

            while (true)
            {
                try
                {
                    await collection.InsertOneAsync(record);

                    return record;
                }
                catch (MongoException ex)
                {
                    var duplicateKeyError = ex.Message.Contains("duplicate key error collection");

                    if (duplicateKeyError && attempt < InsertMaxAttempts)
                    {
                        record.Key = Guid.NewGuid();
                        attempt++;
                    }
                    else if (duplicateKeyError)
                    {
                        throw new RepositoryCustomException(RepositoryCustomError.UnavailableKey);
                    }
                    else
                    {
                        throw ex;
                    }
                }
                catch (TimeoutException ex)
                {
                    throw new RepositoryCustomException(RepositoryCustomError.TimeOutServer(GetMongoServerAddressFromTimeoutException(ex)));
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