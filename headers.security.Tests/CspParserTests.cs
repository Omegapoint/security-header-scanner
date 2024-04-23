using headers.security.Scanner.SecurityConcepts.Csp;

namespace headers.security.Tests;

public class CspParserTests
{
    [Theory]
    [InlineData("default-src 'none' 'self'")]
    public void DoesNotHideIncorrectConfigurations(string csp)
    {
        var policy = new CspPolicy("Merged", csp, true);

        CspParser.GetEffectivePolicy([policy])
            .Should()
            .BeEquivalentTo(policy);
    }

    public static IEnumerable<object[]> MultiplePoliciesWithExpectedMergeResult()
    {
        yield return [
            new[]
            {
                "script-src 'unsafe-eval'",
                "img-src blob:",
                "connect-src 'self'"
            },
            "script-src 'unsafe-eval'; img-src blob:; connect-src 'self'"
        ];
        yield return [
            new[]
            {
                "script-src 'unsafe-eval' 'self'",
                "script-src 'unsafe-eval' *",
                "script-src 'unsafe-eval'"
            },
            "script-src 'unsafe-eval'"
        ];
        yield return [
            new[]
            {
                "script-src *",
                "script-src 'unsafe-eval' 'self'"
            },
            "script-src 'unsafe-eval' 'self'"
        ];
        yield return [
            new[]
            {
                "img-src 'self'",
                "img-src blob:"
            },
            "img-src 'none'"
        ];
        
        // with default-src
        yield return [
            new[]
            {
                "default-src 'self'",
                "script-src 'self'",
                "img-src 'self'",
                "connect-src 'self'"
            },
            "default-src 'self'"
        ];
        yield return [
            new[]
            {
                "default-src 'self'; script-src 'self'; img-src 'self'; connect-src 'self'"
            },
            "default-src 'self'"
        ];
        yield return [
            new[]
            {
                "default-src 'self'",
                "script-src 'self' 'unsafe-eval'",
                "img-src 'self'",
                "connect-src 'self'"
            },
            "default-src 'self'"
        ];
        yield return [
            new[]
            {
                "default-src 'self'; script-src 'self' 'unsafe-eval'; img-src 'self'; connect-src 'self'"
            },
            "default-src 'self'; script-src 'self' 'unsafe-eval'"
        ];
        yield return [
            new[]
            {
                "style-src * 'unsafe-inline'",
                "style-src *"
            },
            "style-src * 'unsafe-inline'"
        ];
    }
    
    [Theory]
    [MemberData(nameof(MultiplePoliciesWithExpectedMergeResult))]
    public void CanMergeMultiplePolicies(string[] csps, string expectedMergedCsp)
    {
        var policies = csps.Select(csp => new CspPolicy("Test", csp, true)).ToList();
        var expectedMergedPolicy = new CspPolicy("Merged", expectedMergedCsp, true);

        CspParser.GetEffectivePolicy(policies)
            .Should()
            .BeEquivalentTo(expectedMergedPolicy);
    }
    
    [Theory]
    [MemberData(nameof(MultiplePoliciesWithExpectedMergeResult))]
    public void OrderDoesNotMatterWhenMergingPolicies(string[] csps, string expectedMergedCsp)
    {
        var policies = csps.Select(csp => new CspPolicy("Test", csp, true)).Reverse().ToList();
        var expectedMergedPolicy = new CspPolicy("Merged", expectedMergedCsp, true);

        CspParser.GetEffectivePolicy(policies)
            .Should()
            .BeEquivalentTo(expectedMergedPolicy);
    }
}