using headers.security.Scanner.Hsts;

namespace headers.security.Tests.HstsPreload;

public class HstsPreloadExampleDataTests
{
    [Fact]
    public async Task CanParseExampleHstsPreloadList()
    {
        var testData = await File.ReadAllTextAsync("./HstsPreload/Data/hstspreload_example_data.2024-04-17.txt.base64");

        var deserialization = () => HstsPreloadClient.DeserializePreloadContent(testData);

        var response = deserialization.Should().NotThrow();

        var treeConstruction = () => PreloadPolicyNode.Create(response.Subject);

        treeConstruction.Should().NotThrow();
    }
}