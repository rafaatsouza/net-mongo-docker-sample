using MongoDockerSample.Core.Domain.Models;
using MongoDockerSample.Core.Domain.Repositories;
using MongoDockerSample.Core.Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDockerSample.Core.Application.Services
{
    public class RegisterService : IRegisterService
    {
        private readonly IRegisterRepository registerRepository;

        public RegisterService(IRegisterRepository registerRepository)
        {
            this.registerRepository = registerRepository
                ?? throw new ArgumentNullException(nameof(registerRepository));
        }

        async Task IRegisterService.DeleteRegisterAsync(Guid key)
            => await registerRepository.DeleteRegisterAsync(key);

        async Task<Register> IRegisterService.GetRegisterAsync(Guid key)
            => await registerRepository.GetRegisterAsync(key);

        async Task<IEnumerable<Register>> IRegisterService.GetRegistersAsync()
            => await registerRepository.GetRegistersAsync();

        async Task<Guid> IRegisterService.InsertRegisterAsync(string value)
            => await registerRepository.InsertRegisterAsync(value);

        async Task IRegisterService.UpdateRegisterAsync(Guid key, string newValue)
            => await registerRepository.UpdateRegisterAsync(key, newValue);
    }
}