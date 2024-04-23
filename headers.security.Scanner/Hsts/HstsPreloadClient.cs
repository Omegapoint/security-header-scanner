using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using headers.security.Scanner.Hsts.Contracts;

namespace headers.security.Scanner.Hsts;

public class HstsPreloadClient(IHttpClientFactory httpClientFactory)
{
    public async Task<List<PreloadEntry>> GetPreloadEntries()
    {
        using var client = httpClientFactory.CreateClient("Integration");

        var response = await client.GetAsync(HstsPreloadConstants.UpstreamUri);

        var base64Content = await response.Content.ReadAsStringAsync();
        var result = DeserializePreloadContent(base64Content);

        return result;
    }

    public static List<PreloadEntry> DeserializePreloadContent(string base64Content)
    {
        var bytes = Convert.FromBase64String(base64Content);
        var str = Encoding.UTF8.GetString(bytes);

        var response = JsonSerializer.Deserialize(str, SourceGenerationContext.Default.PreloadResponse);

        return response.Entries;
    }
}

[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    ReadCommentHandling = JsonCommentHandling.Skip,
    AllowTrailingCommas = true)]
[JsonSerializable(typeof(PreloadResponse))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}