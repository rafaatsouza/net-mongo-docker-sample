using AutoMapper;
using MongoDockerSample.Core.Domain.Repositories;
using MongoDockerSample.Infrastructure.Repository;
using MongoDockerSample.Infrastructure.Repository.Repositories;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RepositoryServiceCollectionExtensions
    {
        public static IServiceCollection AddRepository(
            this IServiceCollection services,
            RepositoryConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            services.AddSingleton(configuration);
            
            services.AddScoped<IEntryRepository, EntryRepository>();

            return services;
        }
    }
}