using MongoDocker.Sample.Domain.Contract.DTO;
using System;
using System.Threading.Tasks;

namespace MongoDocker.Sample.Domain.Service.Interfaces
{
    public interface IMongoDbService
    {
        Task<Guid> InsertValueAsync(string value);
        Task UpdateValueAsync(Guid key, string newValue);
        Task DeleteValueAsync(Guid key);
        Task<MongoDbRegister> GetValueAsync(Guid key);
    }
}