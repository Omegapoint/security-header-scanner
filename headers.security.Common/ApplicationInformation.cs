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
    /// Gets the compile date of the currently executing assembly.
    /// </summary>
    /// <value>The compile date.</value>
    public static DateTime CompileDate => _compileDate ??= RetrieveLinkerTimestamp(ExecutingAssembly.Location);
    
    private static DateTime? _compileDate;

    /// <summary>
    /// Retrieves the linker timestamp.
    /// </summary>
    /// <param name="filePath">The file path.</param>
    /// <returns></returns>
    /// <remarks>http://www.codinghorror.com/blog/2005/04/determining-build-date-the-hard-way.html</remarks>
    private static DateTime RetrieveLinkerTimestamp(string filePath)
    {
        const int peHeaderOffset = 60;
        const int linkerTimestampOffset = 8;
        var b = new byte[2048];
        FileStream s = null;
        try
        {
            s = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            s.Read(b, 0, 2048);
        }
        finally
        {
            s?.Close();
        }

        var seconds = BitConverter.ToInt32(b, BitConverter.ToInt32(b, peHeaderOffset) + linkerTimestampOffset);
        var dt = DateTime.UnixEpoch.AddSeconds(seconds);
        
        return dt.AddHours(TimeZoneInfo.Local.GetUtcOffset(dt).Hours);
    }
}