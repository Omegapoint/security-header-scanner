namespace headers.security.Common.Domain.SecurityConcepts;

public class SimpleSecurityConceptResult(string headerName, List<SecurityConceptResultInfo> infos) : AbstractSecurityConceptResult(infos)
{
    public override string HandlerName { get; } = headerName;
    
    public override string HeaderName { get; } = headerName;
    
    private SecurityGrade FixedSecurityGrade { get; set; } = SecurityGrade.Unknown;
    
    public void SetGrade(SecurityGrade grade) => FixedSecurityGrade = grade;
    
    public override SecurityGrade Grade => FixedSecurityGrade;

    public string MutableValue { get; set; }
    
    public override string ProcessedValue => MutableValue;
}