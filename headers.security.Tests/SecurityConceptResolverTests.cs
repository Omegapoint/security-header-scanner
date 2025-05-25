using System.Reflection;
using headers.security.Scanner.Configuration;
using headers.security.Scanner.Extensions;
using headers.security.Scanner.Hsts;
using headers.security.Scanner.SecurityConcepts;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace headers.security.Tests;

// ReSharper disable once RedundantNameQualifier
[TestSubject(typeof(Scanner.Extensions.ServiceCollectionExtensions))]
public class SecurityConceptResolverTests
{
    [Fact]
    public void RegistersAllSecurityConcepts()
    {
        var hstsPreloadServiceMock = new Mock<IHstsPreloadService>();
        var httpClientConfigurationMock = new Mock<HttpClientConfiguration>();
        
        var services = new ServiceCollection();
        services.AddSingleton(hstsPreloadServiceMock.Object);
        services.AddSingleton(httpClientConfigurationMock.Object);
        services.AddSecurityEngine();

        var sp = services.BuildServiceProvider();

        var registeredSecurityConcepts = sp.GetServices<ISecurityConcept>()
            .Select(s => s.GetType())
            .ToList();
        
        var allSecurityConcepts = Assembly.GetAssembly(typeof(ISecurityConcept))?.GetTypes()
            .Where(t => t.GetInterfaces().Contains(typeof(ISecurityConcept)))
            .ToList();
        
        allSecurityConcepts.Should().HaveCount(9); // I have trust issues...
        registeredSecurityConcepts.Should().BeEquivalentTo(allSecurityConcepts);
    }
}