using headers.security.Common.Domain.SecurityConcepts;
using headers.security.Common.Extensions;
using static headers.security.Common.Domain.SecurityGrade;
using static headers.security.Common.Domain.SecurityImpact;

namespace headers.security.Common.Domain;

public class ScanResult
{
    [Compare]
    public List<ISecurityConceptResult> HandlerResults { get; init; }

    public RawHeaders RawHeaders { get; init; }
    
    public TargetKind TargetKind { get; init; }

    public ScanResult(IEnumerable<ISecurityConceptResult> handlerResults, RawHeaders rawHeaders, TargetKind targetKind)
    {
        HandlerResults = handlerResults.Where(result => result != null).ToList();
        RawHeaders = rawHeaders;
        TargetKind = targetKind;
    }

    public SecurityGrade GetOverallGrade()
    {
        var impacts = HandlerResults
            .Select(result => result.Impact)
            // todo: grading decision: if no handler results exist (this should in practice never happen), give a grade that isn't perfect?
            .DefaultIfEmpty(Medium);
        
        var counts = impacts
            .ToCounter();

        if (counts[Critical] > 0)                   return F;
        if (counts[High] > 1)                       return E;
        if (counts[High] > 0)                       return D;
        if (counts[Medium] > 1)                     return C;
        if (counts[Medium] == 1 || counts[Low] > 1) return B;
        if (counts[Low] == 1)                       return A;
        if (counts[Low] == 0)                       return APlus;
        
        // TODO: we should never end up here, add some testing for this
        return Unknown;
    }
}