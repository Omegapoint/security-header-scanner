using headers.security.Common.Domain.SecurityConcepts;
using static headers.security.Common.Domain.SecurityGrade;

namespace headers.security.Common.Domain;

public class ScanResult
{
    [Compare]
    public List<ISecurityConceptResult> HandlerResults { get; init; }

    public Dictionary<string, List<string>> RawHeaders { get; init; }

    public ScanResult(IEnumerable<ISecurityConceptResult> handlerResults, Dictionary<string, List<string>> rawHeaders)
    {
        HandlerResults = handlerResults.Where(result => result != null).ToList();
        RawHeaders = rawHeaders;
    }

    public SecurityGrade GetOverallGrade() => HandlerResults
        .Select(result => result.Grade)
        // If no handler results exist (this should in practice never happen), give a grade that isn't perfect
        .DefaultIfEmpty(B)
        .Where(grade => grade is >= F and <= A)
        .Min();
}