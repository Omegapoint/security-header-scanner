using System.Diagnostics.CodeAnalysis;
using headers.security.Common.Constants;

namespace headers.security.Tests;

[SuppressMessage("ReSharper", "InvokeAsExtensionMethod")]
public class CspBypassUriTests
{
    public static IEnumerable<object[]> AllKnownBypassUris() => KnownCspBypassUris.All
        .Select(verbatimUri => (object[])[verbatimUri]);

    [Theory]
    [MemberData(nameof(AllKnownBypassUris))]
    public void MatchesKnownBypassUrisExactly(string token)
    {
        KnownCspBypassUris.Matches(token).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(AllKnownBypassUris))]
    public void MatchesPartialKnownBypassUris(string token)
    {
        var domainToken = token.Split('/').First();
        KnownCspBypassUris.Matches(domainToken).Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(AllKnownBypassUris))]
    public void MatchesWildcardKnownBypassUris(string token)
    {
        var wildcardToken = "*." + token.Split('.').Last();
        KnownCspBypassUris.Matches(wildcardToken).Should().BeTrue();
        
        var wildcardFullToken = "*." + token;
        KnownCspBypassUris.Matches(wildcardFullToken).Should().BeTrue();
    }

    [Theory]
    [InlineData("example.com")]
    [InlineData("http://example.com")]
    [InlineData("https://example.com")]
    [InlineData("HTTPS://example.com")]
    [InlineData("le.com")]
    [InlineData("*.le.com")]
    [InlineData("safe.google.com")]
    [InlineData("*.safe.google.com")]
    public void DoesNotMatchSafeTarget(string token)
    {
        KnownCspBypassUris.Matches(token).Should().BeFalse();
    }

    [Theory]
    [InlineData("*")]
    [InlineData("http://*")]
    [InlineData("https://*")]
    [InlineData("HTTPS://*")]
    [InlineData("*.com")]
    [InlineData("google.com")]
    [InlineData("*.google.com")]
    public void MatchesKnownBadTargets(string token)
    {
        KnownCspBypassUris.Matches(token).Should().BeTrue();
    }
}