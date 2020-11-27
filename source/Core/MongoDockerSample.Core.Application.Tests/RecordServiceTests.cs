using MongoDockerSample.Core.Application.Services;
using MongoDockerSample.Core.Domain.Exceptions;
using MongoDockerSample.Core.Domain.Exceptions.Custom;
using MongoDockerSample.Core.Domain.Repositories;
using MongoDockerSample.Core.Domain.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace MongoDockerSample.Core.Application.Tests
{
    public class RecordServiceTests
    {
        private readonly IRecordService recordService;
        private readonly Mock<IRecordRepository> recordRepositoryMock;

        public RecordServiceTests()
        {
            recordRepositoryMock = new Mock<IRecordRepository>();

            recordService = new RecordService(recordRepositoryMock.Object);
        }

        [Fact]
        [Trait(nameof(IRecordService.InsertRecordAsync), "Success")]
        public async Task InsertRecordAsync_Sucess()
        {
            var value = "test";

            await recordService.InsertRecordAsync(value);

            recordRepositoryMock.Verify(r => r.InsertRecordAsync(It.IsAny<string>()));
        }

        [Fact]
        [Trait(nameof(IRecordService.InsertRecordAsync), "Error_ValueNullOrEmpty")]
        public async Task InsertRecordAsync_Error_ValueNullOrEmpty()
        {
            var result = await Assert.ThrowsAsync<RecordCustomException>(async () =>
            {
                await recordService.InsertRecordAsync("");
            });

            Assert.Equal(RecordCustomError.ValueNotInformed.StatusCode, result.StatusCode);
            Assert.Equal(RecordCustomError.ValueNotInformed.Message, result.Message);
        }

        [Fact]
        [Trait(nameof(IRecordService.UpdateRecordAsync), "Success")]
        public async Task UpdateRecordAsync_Sucess()
        {
            var key = Guid.NewGuid();
            var value = "test";

            recordRepositoryMock
                .Setup(r => r.UpdateRecordAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .Callback<Guid, string>((keyCallback, valueCallback) =>
                {
                    Assert.Equal(key, keyCallback);
                    Assert.Equal(value, valueCallback);
                })
                .ReturnsAsync(1);

            await recordService.UpdateRecordAsync(key, value);
        }

        [Fact]
        [Trait(nameof(IRecordService.UpdateRecordAsync), "Error_EmptyKey")]
        public async Task UpdateRecordAsync_Error_EmptyKey()
        {
            var key = Guid.Empty;
            var value = "test";

            var result = await Assert.ThrowsAsync<RecordCustomException>(async () =>
            {
                await recordService.UpdateRecordAsync(key, value);
            });

            Assert.Equal(RecordCustomError.KeyNotInformed.StatusCode, result.StatusCode);
            Assert.Equal(RecordCustomError.KeyNotInformed.Message, result.Message);
        }

        [Fact]
        [Trait(nameof(IRecordService.UpdateRecordAsync), "Error_ValueNullOrEmpty")]
        public async Task UpdateRecordAsync_Error_ValueNullOrEmpty()
        {
            var key = Guid.NewGuid();
            var value = "";

            var result = await Assert.ThrowsAsync<RecordCustomException>(async () =>
            {
                await recordService.UpdateRecordAsync(key, value);
            });

            Assert.Equal(RecordCustomError.ValueNotInformed.StatusCode, result.StatusCode);
            Assert.Equal(RecordCustomError.ValueNotInformed.Message, result.Message);
        }

        [Fact]
        [Trait(nameof(IRecordService.UpdateRecordAsync), "Error_RecordNotFound")]
        public async Task UpdateRecordAsync_Error_RecordNotFound()
        {
            var key = Guid.NewGuid();
            var value = "test";

            recordRepositoryMock
                .Setup(r => r.UpdateRecordAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .Callback<Guid, string>((keyCallback, valueCallback) =>
                {
                    Assert.Equal(key, keyCallback);
                    Assert.Equal(value, valueCallback);
                })
                .ReturnsAsync(0);

            var result = await Assert.ThrowsAsync<RecordCustomException>(async () =>
            {
                await recordService.UpdateRecordAsync(key, value);
            });

            Assert.Equal(RecordCustomError.RecordNotFound.StatusCode, result.StatusCode);
            Assert.Equal(RecordCustomError.RecordNotFound.Message, result.Message);
        }

        [Fact]
        [Trait(nameof(IRecordService.DeleteRecordAsync), "Success")]
        public async Task DeleteRecordAsync_Sucess()
        {
            var key = Guid.NewGuid();

            recordRepositoryMock
                .Setup(r => r.DeleteRecordAsync(It.IsAny<Guid>()))
                .Callback<Guid>((keyCallback) =>
                {
                    Assert.Equal(key, keyCallback);
                })
                .ReturnsAsync(1);

            await recordService.DeleteRecordAsync(key);
        }

        [Fact]
        [Trait(nameof(IRecordService.DeleteRecordAsync), "Error_EmptyKey")]
        public async Task DeleteRecordAsync_Error_EmptyKey()
        {
            var key = Guid.Empty;

            var result = await Assert.ThrowsAsync<RecordCustomException>(async () =>
            {
                await recordService.DeleteRecordAsync(key);
            });

            Assert.Equal(RecordCustomError.KeyNotInformed.StatusCode, result.StatusCode);
            Assert.Equal(RecordCustomError.KeyNotInformed.Message, result.Message);
        }

        [Fact]
        [Trait(nameof(IRecordService.DeleteRecordAsync), "Error_RecordNotFound")]
        public async Task DeleteRecordAsync_Error_RecordNotFound()
        {
            var key = Guid.NewGuid();

            recordRepositoryMock
                .Setup(r => r.DeleteRecordAsync(It.IsAny<Guid>()))
                .Callback<Guid>((keyCallback) =>
                {
                    Assert.Equal(key, keyCallback);
                })
                .ReturnsAsync(0);

            var result = await Assert.ThrowsAsync<RecordCustomException>(async () =>
            {
                await recordService.DeleteRecordAsync(key);
            });

            Assert.Equal(RecordCustomError.RecordNotFound.StatusCode, result.StatusCode);
            Assert.Equal(RecordCustomError.RecordNotFound.Message, result.Message);
        }

        [Fact]
        [Trait(nameof(IRecordService.GetRecordAsync), "Success")]
        public async Task GetRecordAsync_Sucess()
        {
            var key = Guid.NewGuid();
            var value = "teste";

            recordRepositoryMock
                .Setup(r => r.GetRecordAsync(It.IsAny<Guid>()))
                .Callback<Guid>((keyCallback) =>
                {
                    Assert.Equal(key, keyCallback);
                })
                .ReturnsAsync(new Domain.Models.Record()
                {
                    Key = key,
                    Value = value
                    
                });

            var result = await recordService.GetRecordAsync(key);

            Assert.NotNull(result);
            Assert.Equal(key, result.Key);
            Assert.Equal(value, result.Value);
        }

        [Fact]
        [Trait(nameof(IRecordService.GetRecordAsync), "Error_EmptyKey")]
        public async Task GetRecordAsync_Error_EmptyKey()
        {
            var key = Guid.Empty;

            var result = await Assert.ThrowsAsync<RecordCustomException>(async () =>
            {
                await recordService.GetRecordAsync(key);
            });

            Assert.Equal(RecordCustomError.KeyNotInformed.StatusCode, result.StatusCode);
            Assert.Equal(RecordCustomError.KeyNotInformed.Message, result.Message);
        }
        
        [Fact]
        [Trait(nameof(IRecordService.GetRecordAsync), "Error_RecordNotFound")]
        public async Task GetRecordAsync_Error_RecordNotFound()
        {
            var key = Guid.NewGuid();

            recordRepositoryMock
                .Setup(r => r.GetRecordAsync(It.IsAny<Guid>()))
                .Callback<Guid>((keyCallback) =>
                {
                    Assert.Equal(key, keyCallback);
                })
                .ReturnsAsync((Domain.Models.Record)null);

            var result = await Assert.ThrowsAsync<RecordCustomException>(async () =>
            {
                await recordService.GetRecordAsync(key);
            });

            Assert.Equal(RecordCustomError.RecordNotFound.StatusCode, result.StatusCode);
            Assert.Equal(RecordCustomError.RecordNotFound.Message, result.Message);
        }

        [Fact]
        [Trait(nameof(IRecordService.GetRecordsAsync), "Success")]
        public async Task GetRecordsAsync_Sucess()
        {
            var records = new List<Domain.Models.Record>()
            {
                new Domain.Models.Record()
                {
                    Key = Guid.NewGuid(),
                    Value = "test"
                }
            };

            recordRepositoryMock
                .Setup(r => r.GetRecordsAsync())
                .ReturnsAsync(records);
            
            var result = await recordService.GetRecordsAsync();

            Assert.NotNull(result);
            Assert.Equal(records.Count, result.Count());

            for (var i = 0; i < records.Count; i++)
            {
                Assert.Equal(records[i].Key, result.ToList()[i].Key);
                Assert.Equal(records[i].Value, result.ToList()[i].Value);
            }
        }

        [Fact]
        [Trait(nameof(IRecordService.GetRecordsAsync), "Error_RecordNotFound")]
        public async Task GetRecordsAsync_Error_RecordNotFound()
        {
            recordRepositoryMock
                .Setup(r => r.GetRecordsAsync())
                .ReturnsAsync(new List<Domain.Models.Record>());

            var result = await Assert.ThrowsAsync<RecordCustomException>(async () =>
            {
                await recordService.GetRecordsAsync();
            });

            Assert.Equal(RecordCustomError.RecordNotFound.StatusCode, result.StatusCode);
            Assert.Equal(RecordCustomError.RecordNotFound.Message, result.Message);
        }
    }
}