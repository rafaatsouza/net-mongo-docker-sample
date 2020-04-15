using MongoDockerSample.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDockerSample.Core.Domain.Services
{
    public interface IRegisterService
    {
        /// <summary>
        /// Inserts new register
        /// </summary>
        /// <param name="value">String value for the new register</param>
        /// <returns>New register's identifier</returns>
        Task<Guid> InsertRegisterAsync(string value);

        /// <summary>
        /// Update some register's value
        /// </summary>
        /// <param name="key">StRegister's identifier</param>
        /// <param name="newValue">Register's new value</param>
        Task UpdateRegisterAsync(Guid key, string newValue);

        /// <summary>
        /// Removes some register from collection
        /// </summary>
        /// <param name="key">StRegister's identifier</param>
        Task DeleteRegisterAsync(Guid key);

        /// <summary>
        /// Gets register value
        /// </summary>
        /// <param name="key">Register's key</param>
        /// <returns>Registers object <see cref="Register"/></returns>
        Task<Register> GetRegisterAsync(Guid key);

        /// <summary>
        /// Gets all register values
        /// </summary>
        /// <returns>Registers list <see cref="Register"/></returns>
        Task<IEnumerable<Register>> GetRegistersAsync();
    }
}
