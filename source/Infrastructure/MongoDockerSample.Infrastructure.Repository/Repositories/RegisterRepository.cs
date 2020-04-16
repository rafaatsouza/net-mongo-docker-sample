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
    public class RegisterRepository : IRegisterRepository
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
        public RegisterRepository(IMapper mapper, RepositoryConfiguration configurationValues)
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

        async Task<Guid> IRegisterRepository.InsertRegisterAsync(string value)
        {
            var collection = GetMongoCollection();
            var register = await InsertValueAsync(value, collection);

            return register.Key;
        }

        async Task IRegisterRepository.UpdateRegisterAsync(Guid key, string newValue)
        {
            if (key == Guid.Empty)
            {
                throw new RepositoryCustomException(RepositoryCustomError.KeyNotInformed);
            }

            var collection = GetMongoCollection();

            var filter = Builders<RegisterDto>.Filter.Eq(m => m.Key, key);
            var update = Builders<RegisterDto>.Update.Set(m => m.Value, newValue);

            try
            {
                var updateResult = await collection.UpdateOneAsync(filter, update);

                if (updateResult.IsModifiedCountAvailable && updateResult.ModifiedCount == 0)
                {
                    throw new RepositoryCustomException(RepositoryCustomError.RegisterNotFound);
                }
            }
            catch (TimeoutException ex)
            {
                throw new RepositoryCustomException(RepositoryCustomError.TimeOutServer(GetMongoServerAddressFromTimeoutException(ex)));
            }
        }

        async Task IRegisterRepository.DeleteRegisterAsync(Guid key)
        {
            if (key == Guid.Empty)
            {
                throw new RepositoryCustomException(RepositoryCustomError.KeyNotInformed);
            }

            var collection = GetMongoCollection();

            try
            {
                var deletedObject = await collection.FindOneAndDeleteAsync(x => x.Key == key);

                if (deletedObject == null)
                {
                    throw new RepositoryCustomException(RepositoryCustomError.RegisterNotFound);
                }
            }
            catch (TimeoutException ex)
            {
                throw new RepositoryCustomException(RepositoryCustomError.TimeOutServer(GetMongoServerAddressFromTimeoutException(ex)));
            }
        }        

        async Task<Register> IRegisterRepository.GetRegisterAsync(Guid key)
        {
            if (key == Guid.Empty)
            {
                throw new RepositoryCustomException(RepositoryCustomError.KeyNotInformed);
            }

            var collection = GetMongoCollection();

            try
            {
                var filter = Builders<RegisterDto>.Filter.Eq(m => m.Key, key);
                var result = (await collection.FindAsync(filter)).ToList();

                if (!result.Any())
                {
                    throw new RepositoryCustomException(RepositoryCustomError.RegisterNotFound);
                }

                return mapper.Map<Register>(result.FirstOrDefault());
            }
            catch (TimeoutException ex)
            {
                throw new RepositoryCustomException(RepositoryCustomError.TimeOutServer(GetMongoServerAddressFromTimeoutException(ex)));
            }
        }

        async Task<IEnumerable<Register>> IRegisterRepository.GetRegistersAsync()
        {
            var collection = GetMongoCollection();

            try
            {
                var registers = await collection.AsQueryable().ToListAsync();

                return mapper.Map<List<Register>>(registers);
            }
            catch (TimeoutException ex)
            {
                throw new RepositoryCustomException(RepositoryCustomError.TimeOutServer(GetMongoServerAddressFromTimeoutException(ex)));
            }
        }

        private IMongoCollection<RegisterDto> GetMongoCollection()
        {
            var database = mongoClient.GetDatabase(mongoDatabaseName);

            try
            {
                return database.GetCollection<RegisterDto>(collectionName);
            }
            catch (TimeoutException ex)
            {
                throw new RepositoryCustomException(RepositoryCustomError.TimeOutServer(GetMongoServerAddressFromTimeoutException(ex)));
            }
        }

        private async Task<RegisterDto> InsertValueAsync(string value, IMongoCollection<RegisterDto> collection)
        {
            var register = new RegisterDto(value);
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