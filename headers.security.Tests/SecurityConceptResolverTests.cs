using System.Reflection;
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
        
        var services = new ServiceCollection();
        services.AddSingleton(hstsPreloadServiceMock.Object);
        services.AddSecurityEngine();

        var sp = services.BuildServiceProvider();

        var registeredSecurityConcepts = sp.GetServices<ISecurityConcept>().Select(s => s.GetType());
        
        var allSecurityConcepts = Assembly.GetAssembly(typeof(ISecurityConcept))?.GetTypes()
            .Where(t => t.GetInterfaces().Contains(typeof(ISecurityConcept)))
            .ToList();
        
        registeredSecurityConcepts.Should().BeEquivalentTo(allSecurityConcepts);
    }
}