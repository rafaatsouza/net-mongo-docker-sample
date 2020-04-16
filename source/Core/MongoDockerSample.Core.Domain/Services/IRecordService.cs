﻿using MongoDockerSample.Core.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDockerSample.Core.Domain.Services
{
    public interface IRecordService
    {
        /// <summary>
        /// Inserts new record
        /// </summary>
        /// <param name="value">String value for the new record</param>
        /// <returns>New record's identifier</returns>
        Task<Guid> InsertRecordAsync(string value);

        /// <summary>
        /// Update some record's value
        /// </summary>
        /// <param name="key">Record's identifier</param>
        /// <param name="newValue">Record's new value</param>
        Task UpdateRecordAsync(Guid key, string newValue);

        /// <summary>
        /// Removes some record from collection
        /// </summary>
        /// <param name="key">Record's identifier</param>
        Task DeleteRecordAsync(Guid key);

        /// <summary>
        /// Gets record value
        /// </summary>
        /// <param name="key">Record's key</param>
        /// <returns>Records object <see cref="Record"/></returns>
        Task<Record> GetRecordAsync(Guid key);

        /// <summary>
        /// Gets all record values
        /// </summary>
        /// <returns>Records list <see cref="Record"/></returns>
        Task<IEnumerable<Record>> GetRecordsAsync();
    }
}
