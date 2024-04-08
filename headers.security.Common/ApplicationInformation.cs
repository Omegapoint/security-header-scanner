using System.Reflection;

namespace headers.security.Common;

public static class ApplicationInformation
{
    /// <summary>
    /// Gets the executing assembly.
    /// </summary>
    /// <value>The executing assembly.</value>
    public static Assembly ExecutingAssembly => _executingAssembly ??= Assembly.GetExecutingAssembly();

    private static Assembly _executingAssembly;

    /// <summary>
    /// Gets the executing assembly version.
    /// </summary>
    /// <value>The executing assembly version.</value>
    public static Version ExecutingAssemblyVersion => _executingAssemblyVersion ??= ExecutingAssembly.GetName().Version;

    private static Version _executingAssemblyVersion;

    /// <summary>
    /// Gets FileInfo for the currently executing assembly.
    /// </summary>
    /// <value>The compile date.</value>
    public static FileInfo FileInfo => _fileInfo ??= new FileInfo(ExecutingAssembly.Location);
    
    private static FileInfo _fileInfo;

    /// <summary>
    /// Gets the compile date of the currently executing assembly.
    /// </summary>
    /// <value>The compile date.</value>
    public static DateTime CompileDate => _compileDate ??= FileInfo.CreationTimeUtc;
    
    private static DateTime? _compileDate;
}