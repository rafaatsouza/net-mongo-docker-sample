using MongoDocker.Sample.Domain.Contract.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDocker.Sample.Domain.Service.Interfaces
{
    /// <summary>
    /// Service responsible for create, request, update and delete a value
    /// </summary>
    public interface IMongoDbService
    {
        /// <summary>
        /// Inserts new register
        /// </summary>
        /// <param name="value">String value for the new register</param>
        /// <returns>New register's identifier</returns>
        Task<Guid> InsertValueAsync(string value);

        /// <summary>
        /// Update some register's value
        /// </summary>
        /// <param name="key">StRegister's identifier</param>
        /// <param name="newValue">Register's new value</param>
        Task UpdateValueAsync(Guid key, string newValue);

        /// <summary>
        /// Removes some register from collection
        /// </summary>
        /// <param name="key">StRegister's identifier</param>
        Task DeleteValueAsync(Guid key);

        /// <summary>
        /// Gets register value
        /// </summary>
        /// <param name="key">Register's key</param>
        /// <returns>Registers object <see cref="MongoDbRegister"/></returns>
        Task<MongoDbRegister> GetValueAsync(Guid key);

        /// <summary>
        /// Gets all register values
        /// </summary>
        /// <returns>Registers list <see cref="MongoDbRegister"/></returns>
        IEnumerable<MongoDbRegister> GetValues();
    }
}