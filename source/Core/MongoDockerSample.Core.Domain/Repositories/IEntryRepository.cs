using MongoDockerSample.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDockerSample.Core.Domain.Repositories
{
    public interface IEntryRepository
    {
        /// <summary>
        /// Inserts new entry
        /// </summary>
        /// <param name="value">String value for the new entry</param>
        /// <returns>New entry's identifier</returns>
        Task<Guid> InsertEntryAsync(string value);

        /// <summary>
        /// Update some entry's value
        /// </summary>
        /// <param name="key">Entry's identifier</param>
        /// <param name="newValue">Entry's new value</param>
        /// <returns>Updated entries count</returns>
        Task<int> UpdateEntryAsync(Guid key, string newValue);

        /// <summary>
        /// Removes some entry from collection
        /// </summary>
        /// <param name="key">Entry's identifier</param>
        /// <returns>Deleted entries count</returns>
        Task<int> DeleteEntryAsync(Guid key);

        /// <summary>
        /// Gets entry value
        /// </summary>
        /// <param name="key">Entry's key</param>
        /// <returns>Entry object <see cref="Entry"/></returns>
        Task<Entry> GetEntryAsync(Guid key);

        /// <summary>
        /// Gets all entry values
        /// </summary>
        /// <returns>Entries list <see cref="Entry"/></returns>
        Task<ICollection<Entry>> GetEntriesAsync();
    }
}