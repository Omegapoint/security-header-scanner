using headers.security.Scanner;
using headers.security.Scanner.Helpers;
using headers.security.Scanner.SecurityConcepts;

namespace headers.security.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSecurityEngine(this IServiceCollection services)
    {
        var securityConcepts = SecurityConceptResolver.GetSecurityConcepts();

        foreach (var securityConcept in securityConcepts)
        {
            services.Add(new ServiceDescriptor(typeof(ISecurityConcept), securityConcept, ServiceLifetime.Transient));
        }

        services.AddTransient(s => s.GetServices<ISecurityConcept>().ToList());
        services.AddTransient<SecurityEngine>();

        return services;
    }
}