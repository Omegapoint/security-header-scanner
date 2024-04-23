using headers.security.Common.Domain.SecurityConcepts;

namespace headers.security.Scanner.SecurityConcepts;

public interface ISecurityConcept
{
    Task<ISecurityConceptResult> ExecuteAsync(ScanData scanData);
}