namespace headers.security.Scanner.Extensions;

public static class HttpContentExtensions
{
    public static async Task<string> ReadAtMostAsync(this HttpContent content, int maxBuffer)
    {
        using var reader = new StreamReader(await content.ReadAsStreamAsync());
        
        var chars = new char[maxBuffer];
        var read = await reader.ReadBlockAsync(chars, 0, chars.Length);
            
        return read == maxBuffer ? string.Empty : new string(chars, 0, read);
    }
}