using MongoDockerSample.Core.Application.Services;
using MongoDockerSample.Core.Domain.Exceptions;
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
    public class EntryServiceTests
    {
        private readonly IEntryService entryService;
        private readonly Mock<IEntryRepository> entryRepositoryMock;

        public EntryServiceTests()
        {
            entryRepositoryMock = new Mock<IEntryRepository>();

            entryService = new EntryService(entryRepositoryMock.Object);
        }

        [Fact]
        [Trait(nameof(IEntryService.InsertEntryAsync), "Success")]
        public async Task InsertEntryAsync_Sucess()
        {
            var value = "test";

            await entryService.InsertEntryAsync(value);

            entryRepositoryMock.Verify(r => r.InsertEntryAsync(It.IsAny<string>()));
        }

        [Fact]
        [Trait(nameof(IEntryService.InsertEntryAsync), "Error_ValueNullOrEmpty")]
        public async Task InsertEntryAsync_Error_ValueNullOrEmpty()
        {
            var result = await Assert.ThrowsAsync<EntryCustomException>(async () =>
            {
                await entryService.InsertEntryAsync("");
            });

            Assert.Equal(EntryCustomError.ValueNotInformed.StatusCode, result.StatusCode);
            Assert.Equal(EntryCustomError.ValueNotInformed.Message, result.Message);
        }

        [Fact]
        [Trait(nameof(IEntryService.UpdateEntryAsync), "Success")]
        public async Task UpdateEntryAsync_Sucess()
        {
            var key = Guid.NewGuid();
            var value = "test";

            entryRepositoryMock
                .Setup(r => r.UpdateEntryAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .Callback<Guid, string>((keyCallback, valueCallback) =>
                {
                    Assert.Equal(key, keyCallback);
                    Assert.Equal(value, valueCallback);
                })
                .ReturnsAsync(1);

            await entryService.UpdateEntryAsync(key, value);
        }

        [Fact]
        [Trait(nameof(IEntryService.UpdateEntryAsync), "Error_EmptyKey")]
        public async Task UpdateEntryAsync_Error_EmptyKey()
        {
            var key = Guid.Empty;
            var value = "test";

            var result = await Assert.ThrowsAsync<EntryCustomException>(async () =>
            {
                await entryService.UpdateEntryAsync(key, value);
            });

            Assert.Equal(EntryCustomError.KeyNotInformed.StatusCode, result.StatusCode);
            Assert.Equal(EntryCustomError.KeyNotInformed.Message, result.Message);
        }

        [Fact]
        [Trait(nameof(IEntryService.UpdateEntryAsync), "Error_ValueNullOrEmpty")]
        public async Task UpdateEntryAsync_Error_ValueNullOrEmpty()
        {
            var key = Guid.NewGuid();
            var value = "";

            var result = await Assert.ThrowsAsync<EntryCustomException>(async () =>
            {
                await entryService.UpdateEntryAsync(key, value);
            });

            Assert.Equal(EntryCustomError.ValueNotInformed.StatusCode, result.StatusCode);
            Assert.Equal(EntryCustomError.ValueNotInformed.Message, result.Message);
        }

        [Fact]
        [Trait(nameof(IEntryService.UpdateEntryAsync), "Error_RecordNotFound")]
        public async Task UpdateEntryAsync_Error_RecordNotFound()
        {
            var key = Guid.NewGuid();
            var value = "test";

            entryRepositoryMock
                .Setup(r => r.UpdateEntryAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                .Callback<Guid, string>((keyCallback, valueCallback) =>
                {
                    Assert.Equal(key, keyCallback);
                    Assert.Equal(value, valueCallback);
                })
                .ReturnsAsync(0);

            var result = await Assert.ThrowsAsync<EntryCustomException>(async () =>
            {
                await entryService.UpdateEntryAsync(key, value);
            });

            Assert.Equal(EntryCustomError.RecordNotFound.StatusCode, result.StatusCode);
            Assert.Equal(EntryCustomError.RecordNotFound.Message, result.Message);
        }

        [Fact]
        [Trait(nameof(IEntryService.DeleteEntryAsync), "Success")]
        public async Task DeleteEntryAsync_Sucess()
        {
            var key = Guid.NewGuid();

            entryRepositoryMock
                .Setup(r => r.DeleteEntryAsync(It.IsAny<Guid>()))
                .Callback<Guid>((keyCallback) =>
                {
                    Assert.Equal(key, keyCallback);
                })
                .ReturnsAsync(1);

            await entryService.DeleteEntryAsync(key);
        }

        [Fact]
        [Trait(nameof(IEntryService.DeleteEntryAsync), "Error_EmptyKey")]
        public async Task DeleteEntryAsync_Error_EmptyKey()
        {
            var key = Guid.Empty;

            var result = await Assert.ThrowsAsync<EntryCustomException>(async () =>
            {
                await entryService.DeleteEntryAsync(key);
            });

            Assert.Equal(EntryCustomError.KeyNotInformed.StatusCode, result.StatusCode);
            Assert.Equal(EntryCustomError.KeyNotInformed.Message, result.Message);
        }

        [Fact]
        [Trait(nameof(IEntryService.DeleteEntryAsync), "Error_RecordNotFound")]
        public async Task DeleteEntryAsync_Error_RecordNotFound()
        {
            var key = Guid.NewGuid();

            entryRepositoryMock
                .Setup(r => r.DeleteEntryAsync(It.IsAny<Guid>()))
                .Callback<Guid>((keyCallback) =>
                {
                    Assert.Equal(key, keyCallback);
                })
                .ReturnsAsync(0);

            var result = await Assert.ThrowsAsync<EntryCustomException>(async () =>
            {
                await entryService.DeleteEntryAsync(key);
            });

            Assert.Equal(EntryCustomError.RecordNotFound.StatusCode, result.StatusCode);
            Assert.Equal(EntryCustomError.RecordNotFound.Message, result.Message);
        }

        [Fact]
        [Trait(nameof(IEntryService.GetEntryAsync), "Success")]
        public async Task GetEntryAsync_Sucess()
        {
            var key = Guid.NewGuid();
            var value = "teste";

            entryRepositoryMock
                .Setup(r => r.GetEntryAsync(It.IsAny<Guid>()))
                .Callback<Guid>((keyCallback) =>
                {
                    Assert.Equal(key, keyCallback);
                })
                .ReturnsAsync(new Domain.Models.Entry(key, value));

            var result = await entryService.GetEntryAsync(key);

            Assert.NotNull(result);
            Assert.Equal(key, result.Key);
            Assert.Equal(value, result.Value);
        }

        [Fact]
        [Trait(nameof(IEntryService.GetEntryAsync), "Error_EmptyKey")]
        public async Task GetEntryAsync_Error_EmptyKey()
        {
            var key = Guid.Empty;

            var result = await Assert.ThrowsAsync<EntryCustomException>(async () =>
            {
                await entryService.GetEntryAsync(key);
            });

            Assert.Equal(EntryCustomError.KeyNotInformed.StatusCode, result.StatusCode);
            Assert.Equal(EntryCustomError.KeyNotInformed.Message, result.Message);
        }
        
        [Fact]
        [Trait(nameof(IEntryService.GetEntryAsync), "Error_RecordNotFound")]
        public async Task GetEntryAsync_Error_RecordNotFound()
        {
            var key = Guid.NewGuid();

            entryRepositoryMock
                .Setup(r => r.GetEntryAsync(It.IsAny<Guid>()))
                .Callback<Guid>((keyCallback) =>
                {
                    Assert.Equal(key, keyCallback);
                })
                .ReturnsAsync((Domain.Models.Entry)null);

            var result = await Assert.ThrowsAsync<EntryCustomException>(async () =>
            {
                await entryService.GetEntryAsync(key);
            });

            Assert.Equal(EntryCustomError.RecordNotFound.StatusCode, result.StatusCode);
            Assert.Equal(EntryCustomError.RecordNotFound.Message, result.Message);
        }

        [Fact]
        [Trait(nameof(IEntryService.GetEntriesAsync), "Success")]
        public async Task GetEntriesAsync_Sucess()
        {
            var records = new List<Domain.Models.Entry>()
            {
                new Domain.Models.Entry(Guid.NewGuid(), "test")
            };

            entryRepositoryMock
                .Setup(r => r.GetEntriesAsync())
                .ReturnsAsync(records);
            
            var results = await entryService.GetEntriesAsync();

            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.Equal(records.Count, results.Count());

            Assert.All(records, record => 
            {
                var result = results
                    .SingleOrDefault(r => r.Key == record.Key);

                Assert.NotNull(result);

                Assert.Equal(record.Key, result.Key);
                Assert.Equal(record.Value, result.Value);
            });
        }

        [Fact]
        [Trait(nameof(IEntryService.GetEntriesAsync), "Error_RecordNotFound")]
        public async Task GetEntriesAsync_Error_RecordNotFound()
        {
            entryRepositoryMock
                .Setup(r => r.GetEntriesAsync())
                .ReturnsAsync(new List<Domain.Models.Entry>());

            var result = await Assert.ThrowsAsync<EntryCustomException>(async () =>
            {
                await entryService.GetEntriesAsync();
            });

            Assert.Equal(EntryCustomError.RecordNotFound.StatusCode, result.StatusCode);
            Assert.Equal(EntryCustomError.RecordNotFound.Message, result.Message);
        }
    }
}