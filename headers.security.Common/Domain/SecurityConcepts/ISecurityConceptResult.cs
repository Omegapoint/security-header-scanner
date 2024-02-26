namespace headers.security.Common.Domain.SecurityConcepts;

public interface ISecurityConceptResult
{
    string HandlerName { get; }
    
    string HeaderName { get; }
    
    List<SecurityConceptResultInfo> Infos { get; }
    
    SecurityGrade Grade { get; }
    
    object ProcessedValue { get; }
}

public abstract class GenericSecurityConceptResult(List<SecurityConceptResultInfo> infos) : ISecurityConceptResult
{
    public GenericSecurityConceptResult() : this([])
    {
    }

    [Compare]
    public abstract string HandlerName { get; }
    
    [Compare]
    public abstract string HeaderName { get; }

    [Compare]
    public List<SecurityConceptResultInfo> Infos { get; init; } = infos;

    [Compare]
    public abstract SecurityGrade Grade { get; }
    
    [Compare]
    public abstract object ProcessedValue { get; }
}

public class BasicSecurityConceptResult(string headerName, List<SecurityConceptResultInfo> infos) : GenericSecurityConceptResult(infos)
{
    private SecurityGrade FixedSecurityGrade { get; set; } = SecurityGrade.Unknown;
    public string MutableValue { get; set; }

    public void SetGrade(SecurityGrade grade) => FixedSecurityGrade = grade;

    public override SecurityGrade Grade => FixedSecurityGrade;

    public override string HandlerName { get; } = headerName;
    public override string HeaderName { get; } = headerName;

    public override string ProcessedValue => MutableValue;
}