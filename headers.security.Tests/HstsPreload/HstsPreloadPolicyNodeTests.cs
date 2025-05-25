using headers.security.Scanner.Hsts;
using headers.security.Scanner.Hsts.Contracts;

namespace headers.security.Tests.HstsPreload;

public class HstsPreloadPolicyNodeTests
{
    public static IEnumerable<object[]> PreloadEntries => [
        [
            Array.Empty<PreloadEntry>()
        ],
        [
            new[] { new PreloadEntry("example.test") }
        ],
        [
            new[]
            {
                new PreloadEntry("example.test"),
                new PreloadEntry("other-example.test")
            }
        ],
        [
            new[]
            {
                new PreloadEntry("example.test"),
                new PreloadEntry("example.test")
            },
        ]
    ];
    
    [Theory]
    [MemberData(nameof(PreloadEntries))]
    public void CanConstructTreeWithData(PreloadEntry[] entries)
    {
        var treeConstruction = () => PreloadPolicyNode.Create(entries);

        treeConstruction.Should().NotThrow();
    }
    
    public static IEnumerable<object[]> OverwritingPreloadEntriesWithPolicy => [
        [
            new[]
            {
                new PreloadEntry("example.test")
                {
                    IncludeSubdomains = false,
                    Policy = "some-losing-policy"
                },
                new PreloadEntry("example.test")
                {
                    IncludeSubdomains = true,
                    Policy = "some-winning-policy"
                }
            }
        ],
    ];
    
    [Theory]
    [MemberData(nameof(OverwritingPreloadEntriesWithPolicy))]
    public void OverwritesWithLastValue(PreloadEntry[] entries)
    {
        var tree = PreloadPolicyNode.Create(entries);

        var loser = entries.First();
        var winner = entries.Last();

        loser.Should().NotBeEquivalentTo(winner);

        tree.GetOrDefault(winner.Domain).Policy.Should().BeEquivalentTo(winner);
        tree.GetOrDefault(winner.Domain).Policy.Should().NotBeEquivalentTo(loser);
    }
    
    public static IEnumerable<object[]> PreloadEntriesWithSubdomain => [
        [
            new[]
            {
                new PreloadEntry("example.test")
                {
                    IncludeSubdomains = true,
                    Policy = "policy"
                },
                new PreloadEntry("sub.example.test")
                {
                    IncludeSubdomains = true,
                    Policy = "subdomain-policy"
                }
            }
        ],
    ];
    
    [Theory]
    [MemberData(nameof(PreloadEntriesWithSubdomain))]
    public void PrefersExactMatchEvenWhenIncludeSubdomain(PreloadEntry[] entries)
    {
        entries.Should().OnlyContain(p => p.IncludeSubdomains);

        var tree = PreloadPolicyNode.Create(entries);

        var domainPolicy = entries.First();
        var subdomainPolicy = entries.Last();

        subdomainPolicy.Domain.Should().Contain(domainPolicy.Domain);

        tree.GetOrDefault(domainPolicy.Domain).Should().BeEquivalentTo(PreloadMatch.Exact(domainPolicy));
        tree.GetOrDefault(subdomainPolicy.Domain).Should().BeEquivalentTo(PreloadMatch.Exact(subdomainPolicy));
    }
    
    [Theory]
    [MemberData(nameof(PreloadEntriesWithSubdomain))]
    public void ReturnsParentPolicyIfIncludeSubdomain(PreloadEntry[] entries)
    {
        entries.Should().OnlyContain(p => p.IncludeSubdomains);

        var tree = PreloadPolicyNode.Create(entries);

        var domainPolicy = entries.First();
        var subdomainPolicy = entries.Last();

        subdomainPolicy.Domain.Should().Contain(domainPolicy.Domain);

        tree.GetOrDefault("direct." + domainPolicy.Domain).Should().BeEquivalentTo(PreloadMatch.Subdomain(domainPolicy));
        tree.GetOrDefault("direct." + subdomainPolicy.Domain).Should().BeEquivalentTo(PreloadMatch.Subdomain(subdomainPolicy));

        tree.GetOrDefault("deep.child.of." + domainPolicy.Domain).Should().BeEquivalentTo(PreloadMatch.Subdomain(domainPolicy));
        tree.GetOrDefault("deep.child.of." + subdomainPolicy.Domain).Should().BeEquivalentTo(PreloadMatch.Subdomain(subdomainPolicy));
    }
    
    [Theory]
    [MemberData(nameof(PreloadEntriesWithSubdomain))]
    public void AlwaysReturnsMatchObject(PreloadEntry[] entries)
    {
        entries.Should().OnlyContain(p => p.IncludeSubdomains);
        
        var tree = PreloadPolicyNode.Create(entries);

        var domainPolicy = entries.First();
        var subdomainPolicy = entries.Last();

        subdomainPolicy.Domain.Should().Contain(domainPolicy.Domain);

        tree.GetOrDefault(domainPolicy.Domain).Should().BeOfType<PreloadMatch>();
        tree.GetOrDefault(subdomainPolicy.Domain).Should().BeOfType<PreloadMatch>();

        tree.GetOrDefault(domainPolicy.Domain + ".but.with.non-existent.root").Should().BeOfType<PreloadMatch>();
        tree.GetOrDefault(subdomainPolicy.Domain + ".but.with.non-existent.root").Should().BeOfType<PreloadMatch>();

        var emptyTree = PreloadPolicyNode.Create([]);
        emptyTree.GetOrDefault(domainPolicy.Domain).Should().BeOfType<PreloadMatch>();
    }
    
    public static IEnumerable<object[]> PreloadEntriesWithoutSubdomain => [
        [
            new[]
            {
                new PreloadEntry("example.test")
                {
                    IncludeSubdomains = false,
                    Policy = "policy"
                },
                new PreloadEntry("sub.example.test")
                {
                    IncludeSubdomains = false,
                    Policy = "subdomain-policy"
                }
            }
        ],
    ];
    
    [Theory]
    [MemberData(nameof(PreloadEntriesWithoutSubdomain))]
    public void ReturnsOnlyExactMatchIfNotIncludeSubdomain(PreloadEntry[] entries)
    {
        entries.Should().OnlyContain(p => p.IncludeSubdomains == false);
        
        var tree = PreloadPolicyNode.Create(entries);

        var domainPolicy = entries.First();
        var subdomainPolicy = entries.Last();

        subdomainPolicy.Domain.Should().Contain(domainPolicy.Domain);

        tree.GetOrDefault(domainPolicy.Domain).Should().BeEquivalentTo(PreloadMatch.Exact(domainPolicy));
        tree.GetOrDefault(subdomainPolicy.Domain).Should().BeEquivalentTo(PreloadMatch.Exact(subdomainPolicy));

        tree.GetOrDefault("direct." + domainPolicy.Domain).Should().BeEquivalentTo(PreloadMatch.NotMatched());
        tree.GetOrDefault("direct." + subdomainPolicy.Domain).Should().BeEquivalentTo(PreloadMatch.NotMatched());

        tree.GetOrDefault("deep.child.of." + domainPolicy.Domain).Should().BeEquivalentTo(PreloadMatch.NotMatched());
        tree.GetOrDefault("deep.child.of." + subdomainPolicy.Domain).Should().BeEquivalentTo(PreloadMatch.NotMatched());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\n")]
    [InlineData("\t")]
    [InlineData(null)]
    public void ThrowsErrorWhenInvalidStringDomainIsSupplied(string invalidDomain)
    {
        var tree = PreloadPolicyNode.Create([]);

        var action = () => tree.GetOrDefault(invalidDomain);

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    public void ThrowsErrorWhenInvalidUriDomainIsSupplied(Uri invalidDomain)
    {
        var tree = PreloadPolicyNode.Create([]);

        var action = () => tree.GetOrDefault(invalidDomain);

        action.Should().Throw<ArgumentException>();
    }
}