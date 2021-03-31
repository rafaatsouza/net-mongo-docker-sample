using MongoDockerSample.Core.Application.Services;
using MongoDockerSample.Core.Domain.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IEntryService , EntryService>();

            return services;
        }
    }
}