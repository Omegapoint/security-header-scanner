using headers.security.Scanner.SecurityConcepts;
using Microsoft.Extensions.DependencyInjection;

namespace headers.security.Scanner.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddSecurityEngine(this IServiceCollection services)
    {
        services.AddTransient(typeof(ISecurityConcept), typeof(AccessControlAllowOriginSecurityConcept));
        services.AddTransient(typeof(ISecurityConcept), typeof(CacheControlSecurityConcept));
        services.AddTransient(typeof(ISecurityConcept), typeof(CspSecurityConcept));
        services.AddTransient(typeof(ISecurityConcept), typeof(PermissionsPolicySecurityConcept));
        services.AddTransient(typeof(ISecurityConcept), typeof(ReferrerPolicySecurityConcept));
        services.AddTransient(typeof(ISecurityConcept), typeof(ServerSecurityConcept));
        services.AddTransient(typeof(ISecurityConcept), typeof(StrictTransportSecurityConcept));
        services.AddTransient(typeof(ISecurityConcept), typeof(XContentTypeOptionsSecurityConcept));
        services.AddTransient(typeof(ISecurityConcept), typeof(XFrameOptionsSecurityConcept));

        services.AddTransient(s => s.GetServices<ISecurityConcept>().ToList());
        services.AddTransient<SecurityEngine>();
    }
}