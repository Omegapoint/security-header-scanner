using Microsoft.Extensions.DependencyInjection;

namespace CloudLookup.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCloudLookupClient(this IServiceCollection services)
    {
        services.AddSingleton<CloudLookupClient>();

        return services;
    }
}