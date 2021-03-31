using AutoMapper;
using MongoDB.Driver;
using MongoDockerSample.Core.Domain.Models;
using MongoDockerSample.Core.Domain.Repositories;
using MongoDockerSample.Infrastructure.Repository.Dtos;
using MongoDockerSample.Infrastructure.Repository.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDockerSample.Infrastructure.Repository.Repositories
{
    public class EntryRepository : IEntryRepository
    {
        private readonly IMapper mapper;
        private readonly MongoClient mongoClient;
        private readonly string mongoDatabaseName;
        private readonly string collectionName;

        private const int InsertMaxAttempts = 3;
        private const string DuplicatedKeyMongoMessageError = "duplicate key error collection";

        private readonly static ICollection<Entry> emptyEntries
            = Enumerable.Empty<Entry>().ToArray();

        /// <summary>
        /// Receives MongoDb configuration values through dependency injection
        /// </summary>
        /// <param name="configurationValues"></param>
        public EntryRepository(IMapper mapper, 
            RepositoryConfiguration configurationValues)
        {
            this.mapper = mapper 
                ?? throw new ArgumentNullException(nameof(mapper));

            if (configurationValues == null)
                throw new ArgumentNullException(nameof(configurationValues));

            if (string.IsNullOrEmpty(configurationValues.MongoServer))
                throw new ArgumentException("Null or empty", 
                    nameof(configurationValues.MongoServer));

            if (string.IsNullOrEmpty(configurationValues.MongoDatabase))
                throw new ArgumentException("Null or empty", 
                    nameof(configurationValues.MongoDatabase));

            if (string.IsNullOrEmpty(configurationValues.MongoCollection))
                throw new ArgumentException("Null or empty", 
                    nameof(configurationValues.MongoCollection));

            mongoDatabaseName = configurationValues.MongoDatabase;
            collectionName = configurationValues.MongoCollection;

            mongoClient = new MongoClient(configurationValues.MongoServer);
        }

        async Task<Guid> IEntryRepository.InsertEntryAsync(string value)
        {
            var collection = GetMongoCollection();
            var record = await InsertValueAsync(value, collection);

            return record.Key;
        }

        async Task<int> IEntryRepository.UpdateEntryAsync(Guid key, string newValue)
        {
            var collection = GetMongoCollection();

            var filter = Builders<EntryDto>.Filter.Eq(m => m.Key, key);
            var update = Builders<EntryDto>.Update.Set(m => m.Value, newValue);

            try
            {
                var updateResult = await collection
                    .UpdateOneAsync(filter, update);

                return updateResult.IsModifiedCountAvailable 
                    ? (int)updateResult.ModifiedCount : 0;
            }
            catch (TimeoutException)
            {
                throw new RepositoryCustomException(
                    RepositoryCustomError.TimeOutServer);
            }
        }

        async Task<int> IEntryRepository.DeleteEntryAsync(Guid key)
        {
            var collection = GetMongoCollection();

            try
            {
                var deletedRecord = await collection
                    .FindOneAndDeleteAsync(x => x.Key == key);

                return deletedRecord != null ? 1 : 0;
            }
            catch (TimeoutException)
            {
                throw new RepositoryCustomException(
                    RepositoryCustomError.TimeOutServer);
            }
        }        

        async Task<Entry> IEntryRepository.GetEntryAsync(Guid key)
        {
            var collection = GetMongoCollection();

            try
            {
                var filter = Builders<EntryDto>.Filter.Eq(m => m.Key, key);
                var result = (await collection.FindAsync(filter)).ToList();

                if (result == null || !result.Any())
                {
                    return null;
                }

                return mapper.Map<Entry>(result.FirstOrDefault());
            }
            catch (TimeoutException)
            {
                throw new RepositoryCustomException(
                    RepositoryCustomError.TimeOutServer);
            }
        }

        async Task<ICollection<Entry>> IEntryRepository.GetEntriesAsync()
        {
            var collection = GetMongoCollection();

            try
            {
                var records = await collection
                    .AsQueryable().ToListAsync();

                if (records == null || !records.Any())
                {
                    return emptyEntries;
                }

                return mapper.Map<ICollection<Entry>>(records);
            }
            catch (TimeoutException)
            {
                throw new RepositoryCustomException(
                    RepositoryCustomError.TimeOutServer);
            }
        }

        private IMongoCollection<EntryDto> GetMongoCollection()
        {
            try
            {
                var database = mongoClient.GetDatabase(mongoDatabaseName);

                return database.GetCollection<EntryDto>(collectionName);
            }
            catch (TimeoutException)
            {
                throw new RepositoryCustomException(
                    RepositoryCustomError.TimeOutServer);
            }
        }

        private async Task<EntryDto> InsertValueAsync(
            string value, IMongoCollection<EntryDto> collection)
        {
            var record = new EntryDto(value);
            
            for(var attempt = 0; attempt < InsertMaxAttempts; attempt++)
            {
                try
                {
                    await collection.InsertOneAsync(record);

                    return record;
                }
                catch (MongoException ex)
                {
                    var duplicatedKeyError = ex.Message
                        .Contains(DuplicatedKeyMongoMessageError);

                    if (!duplicatedKeyError)
                    {
                        throw;
                    }

                    record.Key = Guid.NewGuid();
                }
                catch (TimeoutException)
                {
                    throw new RepositoryCustomException(
                        RepositoryCustomError.TimeOutServer);
                }
            }

            throw new RepositoryCustomException(
                RepositoryCustomError.UnavailableKey);
        }
    }
}