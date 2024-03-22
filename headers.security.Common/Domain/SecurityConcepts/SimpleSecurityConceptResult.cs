namespace headers.security.Common.Domain.SecurityConcepts;

public class SimpleSecurityConceptResult(string headerName, List<ISecurityConceptResultInfo> infos, SecurityImpact initialSecurityImpact = SecurityImpact.None) : AbstractSecurityConceptResult(infos)
{
    public override string HandlerName { get; } = headerName;
    
    public override string HeaderName { get; } = headerName;
    
    private SecurityImpact FixedSecurityImpact { get; set; } = initialSecurityImpact;
    
    public void SetImpact(SecurityImpact impact) => FixedSecurityImpact = impact;
    
    public override SecurityImpact Impact => FixedSecurityImpact;

    public string StringValue { get; set; }
    
    public override string ProcessedValue => StringValue;
}