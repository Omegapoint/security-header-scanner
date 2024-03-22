namespace headers.security.Common.Domain.SecurityConcepts;

public interface ISecurityConceptResultInfo
{
    string Message { get; init; }
    
    List<List<string>> FormatTokens { get; init; }
    
    Uri ExternalLink { get; init; }
}