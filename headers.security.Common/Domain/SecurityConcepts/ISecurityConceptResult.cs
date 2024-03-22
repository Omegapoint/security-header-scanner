namespace headers.security.Common.Domain.SecurityConcepts;

public interface ISecurityConceptResult
{
    string HandlerName { get; }
    
    string HeaderName { get; }
    
    List<ISecurityConceptResultInfo> Infos { get; }
    
    SecurityImpact Impact { get; }
    
    object ProcessedValue { get; }
}

public abstract class AbstractSecurityConceptResult(List<ISecurityConceptResultInfo> infos) : ISecurityConceptResult
{
    [Compare]
    public abstract string HandlerName { get; }
    
    [Compare]
    public abstract string HeaderName { get; }

    [Compare]
    public List<ISecurityConceptResultInfo> Infos { get; init; } = infos;

    [Compare]
    public abstract SecurityImpact Impact { get; }
    
    [Compare]
    public abstract object ProcessedValue { get; }
}