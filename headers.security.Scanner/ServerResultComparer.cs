using headers.security.Common;
using headers.security.Common.Domain;
using headers.security.Scanner.SecurityConcepts.Csp;
using KellermanSoftware.CompareNetObjects;

namespace headers.security.Scanner;

public static class ServerResultComparer
{
    private static readonly CompareLogic CompareLogic = new()
    {
        Config = new ComparisonConfig
        {
            RequiredAttributesToCompare = [typeof(CompareAttribute)]
        }
    };

    private static readonly List<(string PathContains, string ValueStartsWith)> AcceptableDifferences = 
    [(
        $".{nameof(CspPolicy.Directives)}.[",
        CspParser.NoncePrefix
    ),];

    public static List<ServerResult> MergeEqual(List<(IPWithCloud IP, Uri FinalUri, DateTime FetchedAt, ScanResult Result)> uriResults, Uri targetUri)
    {
        var processed = new List<ScanResult>();
        var results = new List<ServerResult>();

        var fetchedAt = uriResults.Min(r => r.FetchedAt);
        
        foreach (var (ip, finalUri, _, result) in uriResults)
        {
            if (processed.Contains(result))
            {
                continue;
            }

            processed.Add(result);
            HashSet<IPWithCloud> resultIPs = [ip];
                
            foreach (var (otherIp, otherFinalUri, _, otherResult) in uriResults)
            {
                if (processed.Contains(otherResult) || !finalUri.Equals(otherFinalUri) || !AreEqual(result, otherResult))
                {
                    continue;
                }

                processed.Add(otherResult);
                resultIPs.Add(otherIp);
            }

            results.Add(new ServerResult
            {
                RequestTarget = ScanTarget.From(targetUri),
                FinalTarget = ScanTarget.From(finalUri),
                IPs = resultIPs.ToList(),
                Result = result,
                Grade = result.GetOverallGrade(),
                FetchedAt = fetchedAt
            });
        }

        return results;
    }

    private static bool AreEqual(ScanResult result, ScanResult otherResult)
    {
        var comparisonResult = CompareLogic.Compare(result, otherResult);
        
        return comparisonResult.AreEqual || comparisonResult.Differences.All(AcceptableDifference);
    }

    private static bool AcceptableDifference(Difference diff)
    {
        var diffPath = diff.PropertyName;
        var diffValue = diff.Object1Value;
        
        return AcceptableDifferences.Any(
            kvp => diffPath.Contains(kvp.PathContains) && diffValue.StartsWith(kvp.ValueStartsWith)
        );
    }
}