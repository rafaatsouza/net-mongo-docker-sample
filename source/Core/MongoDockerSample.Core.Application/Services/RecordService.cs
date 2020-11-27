using MongoDockerSample.Core.Domain.Exceptions;
using MongoDockerSample.Core.Domain.Models;
using MongoDockerSample.Core.Domain.Repositories;
using MongoDockerSample.Core.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDockerSample.Core.Application.Services
{
    public class RecordService : IRecordService
    {
        private readonly IRecordRepository recordRepository;

        public RecordService(IRecordRepository recordRepository)
        {
            this.recordRepository = recordRepository
                ?? throw new ArgumentNullException(nameof(recordRepository));
        }

        Task IRecordService.DeleteRecordAsync(Guid key)
        {
            ValidateKey(key);

            return ExecuteDeleteRecordAsync(key);
        }

        Task<Record> IRecordService.GetRecordAsync(Guid key)
        {
            ValidateKey(key);

            return ExecuteGetRecordAsync(key);
        }

        async Task<IEnumerable<Record>> IRecordService.GetRecordsAsync() 
        {
            var records = await recordRepository.GetRecordsAsync();

            if (records == null || !records.Any())
            {
                throw new RecordCustomException(
                    RecordCustomError.RecordNotFound);
            }

            return records;
        }
        
        Task<Guid> IRecordService.InsertRecordAsync(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new RecordCustomException(
                    RecordCustomError.ValueNotInformed);

            return recordRepository.InsertRecordAsync(value);
        }

        Task IRecordService.UpdateRecordAsync(
            Guid key, string newValue)
        {
            ValidateKey(key);

            if (string.IsNullOrEmpty(newValue))
                throw new RecordCustomException(
                    RecordCustomError.ValueNotInformed);

            return ExecuteUpdateRecordAsync(key, newValue);
        }

        private async Task ExecuteUpdateRecordAsync(
            Guid key, string newValue)
        {
            var updatedCount = await recordRepository
                .UpdateRecordAsync(key, newValue);

            if (updatedCount == 0)
            {
                throw new RecordCustomException(
                    RecordCustomError.RecordNotFound);
            }
        }

        private async Task ExecuteDeleteRecordAsync(Guid key)
        {
            var deletedCount = await recordRepository
                .DeleteRecordAsync(key);

            if (deletedCount == 0)
            {
                throw new RecordCustomException(
                    RecordCustomError.RecordNotFound);
            }
        }

        private async Task<Record> ExecuteGetRecordAsync(Guid key)
        {
            var record = await recordRepository.GetRecordAsync(key);

            if (record == null)
            {
                throw new RecordCustomException(
                    RecordCustomError.RecordNotFound);
            }

            return record;
        }

        private void ValidateKey(Guid key)
        {
            if (key == null || key == Guid.Empty)
            {
                throw new RecordCustomException(
                    RecordCustomError.KeyNotInformed);
            }
        }
    }
}