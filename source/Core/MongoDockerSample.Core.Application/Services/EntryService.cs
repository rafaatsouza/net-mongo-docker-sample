using MongoDockerSample.Core.Domain.Exceptions;
using MongoDockerSample.Core.Domain.Models;
using MongoDockerSample.Core.Domain.Repositories;
using MongoDockerSample.Core.Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDockerSample.Core.Application.Services
{
    public class EntryService : IEntryService
    {
        private readonly IEntryRepository entryRepository;

        public EntryService(IEntryRepository entryRepository)
        {
            this.entryRepository = entryRepository
                ?? throw new ArgumentNullException(nameof(entryRepository));
        }

        Task IEntryService.DeleteEntryAsync(Guid key)
        {
            ValidateKey(key);

            return ExecuteDeleteEntryAsync(key);
        }

        Task<Entry> IEntryService.GetEntryAsync(Guid key)
        {
            ValidateKey(key);

            return ExecuteGetEntryAsync(key);
        }

        Task<ICollection<Entry>> IEntryService.GetEntriesAsync()
        {
            return entryRepository.GetEntriesAsync();
        }
        
        Task<Guid> IEntryService.InsertEntryAsync(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new EntryCustomException(
                    EntryCustomError.ValueNotInformed);

            return entryRepository.InsertEntryAsync(value);
        }

        Task IEntryService.UpdateEntryAsync(
            Guid key, string newValue)
        {
            ValidateKey(key);

            if (string.IsNullOrEmpty(newValue))
                throw new EntryCustomException(
                    EntryCustomError.ValueNotInformed);

            return ExecuteUpdateEntryAsync(key, newValue);
        }

        private async Task ExecuteUpdateEntryAsync(
            Guid key, string newValue)
        {
            var updatedCount = await entryRepository
                .UpdateEntryAsync(key, newValue);

            if (updatedCount == 0)
            {
                throw new EntryCustomException(
                    EntryCustomError.RecordNotFound);
            }
        }

        private async Task ExecuteDeleteEntryAsync(Guid key)
        {
            var deletedCount = await entryRepository
                .DeleteEntryAsync(key);

            if (deletedCount == 0)
            {
                throw new EntryCustomException(
                    EntryCustomError.RecordNotFound);
            }
        }

        private async Task<Entry> ExecuteGetEntryAsync(Guid key)
        {
            var record = await entryRepository.GetEntryAsync(key);

            if (record == null)
            {
                throw new EntryCustomException(
                    EntryCustomError.RecordNotFound);
            }

            return record;
        }

        private void ValidateKey(Guid key)
        {
            if (key == Guid.Empty)
            {
                throw new EntryCustomException(
                    EntryCustomError.KeyNotInformed);
            }
        }
    }
}