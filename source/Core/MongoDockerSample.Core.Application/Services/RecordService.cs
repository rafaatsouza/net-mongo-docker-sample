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

        async Task IRecordService.DeleteRecordAsync(Guid key)
        {
            if (key == Guid.Empty)
            {
                throw new RecordCustomException(RecordCustomError.KeyNotInformed);
            }

            var deletedCount = await recordRepository.DeleteRecordAsync(key);

            if (deletedCount == 0)
            {
                throw new RecordCustomException(RecordCustomError.RecordNotFound);
            }
        }

        async Task<Record> IRecordService.GetRecordAsync(Guid key)
        {
            if (key == Guid.Empty)
            {
                throw new RecordCustomException(RecordCustomError.KeyNotInformed);
            }

            var record = await recordRepository.GetRecordAsync(key);

            if (record == null)
            {
                throw new RecordCustomException(RecordCustomError.RecordNotFound);
            }

            return record;
        }

        async Task<IEnumerable<Record>> IRecordService.GetRecordsAsync() 
        {
            var records = await recordRepository.GetRecordsAsync();

            if (records == null || !records.Any())
            {
                throw new RecordCustomException(RecordCustomError.RecordNotFound);
            }

            return records;
        }
        
        async Task<Guid> IRecordService.InsertRecordAsync(string value)
            => await recordRepository.InsertRecordAsync(value);

        async Task IRecordService.UpdateRecordAsync(Guid key, string newValue)
        {
            if (key == Guid.Empty)
            {
                throw new RecordCustomException(RecordCustomError.KeyNotInformed);
            }

            var updatedCount = await recordRepository.UpdateRecordAsync(key, newValue);

            if (updatedCount == 0)
            {
                throw new RecordCustomException(RecordCustomError.RecordNotFound);
            }
        }
    }
}