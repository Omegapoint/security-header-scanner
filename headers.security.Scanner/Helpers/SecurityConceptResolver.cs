using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using headers.security.Scanner.SecurityConcepts;

namespace headers.security.Scanner.Helpers;

public static class SecurityConceptResolver
{
    [RequiresUnreferencedCode("Calls System.Reflection.Assembly.GetTypes()")]
    public static List<Type> GetSecurityConcepts() => Assembly.GetExecutingAssembly().GetTypes()
        .Where(t => t.GetInterfaces().Contains(typeof(ISecurityConcept)))
        .ToList();
}