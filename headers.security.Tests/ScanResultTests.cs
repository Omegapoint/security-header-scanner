using headers.security.Common.Domain;
using headers.security.Common.Domain.SecurityConcepts;
using headers.security.Tests.Extensions;

namespace headers.security.Tests;

public class ScanResultTests
{
    public static IEnumerable<object[]> AllScanResults =>
    [
        ..ScanResultsWithNoSecurityImpact(),
        ..ScanResultsWithExpectedGrade().FirstMemberAsArray()
    ];
    
    [Theory]
    [MemberData(nameof(AllScanResults))]
    public void GetOverallGradeNeverReturnsUnknown(ScanResult result)
    {
        result.GetOverallGrade().Should().NotBe(SecurityGrade.Unknown);
    }
    
    public static IEnumerable<object[]> ScanResultsWithNoSecurityImpact()
    {
        yield return [
            new ScanResult([], new(), TargetKind.Detect)
        ];
        yield return [
            new ScanResult([new SimpleSecurityConceptResult("X-TestHeader", [])], new(), TargetKind.Detect)
        ];
        yield return [
            new ScanResult([new SimpleSecurityConceptResult("X-TestHeader", [], SecurityImpact.Info)], new(), TargetKind.Detect)
        ];
    }
    
    [Theory]
    [MemberData(nameof(ScanResultsWithNoSecurityImpact))]
    public void GetOverallGradeReturnsAPlusWhenNoImpactRegistered(ScanResult result)
    {
        result.GetOverallGrade().Should().Be(SecurityGrade.APlus);
    }
    
    public static IEnumerable<object[]> ScanResultsWithExpectedGrade()
    {
        yield return [
            new ScanResult([new SimpleSecurityConceptResult("X-TestHeader", [], SecurityImpact.Critical)], new(), TargetKind.Detect),
            SecurityGrade.F
        ];
        yield return [
            new ScanResult([new SimpleSecurityConceptResult("X-TestHeader", [], SecurityImpact.Low)], new(), TargetKind.Detect),
            SecurityGrade.A
        ];
        yield return [
            new ScanResult([
                new SimpleSecurityConceptResult("X-TestHeader", [], SecurityImpact.Low),
                new SimpleSecurityConceptResult("X-TestHeader2", [], SecurityImpact.Low),
            ], new(), TargetKind.Detect),
            SecurityGrade.B
        ];
    }

    [Theory]
    [MemberData(nameof(ScanResultsWithExpectedGrade))]
    public void GetOverallGradeReturnsExpectedGrade(ScanResult result, SecurityGrade grade)
    {
        result.GetOverallGrade().Should().Be(grade);
    }
}