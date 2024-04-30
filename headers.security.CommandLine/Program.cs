using System.CommandLine;
using headers.security.CachedContent.Extensions;
using headers.security.Scanner;
using headers.security.Scanner.Helpers;
using headers.security.Scanner.SecurityConcepts;
using Microsoft.Extensions.DependencyInjection;

namespace headers.security.CommandLine;

public static class Program
{
    static async Task<int> Main(string[] args)
    {
        var fileOption = new Option<FileSystemInfo>(
            name: "--file",
            description: "An option whose argument is parsed as a FileInfo",
            isDefault: true,
            parseArgument: result =>
            {
                if (!result.Tokens.Any())
                {
                    return null;
                }
                
                var filePath = result.Tokens.Single().Value;
                if (!File.Exists(filePath))
                {
                    result.ErrorMessage = "File does not exist";
                    return null;
                }

                return new FileInfo(filePath);
            });
        
        var jsonOption = new Option<bool>(
            aliases: ["--json", "-j"],
            description: "Return output as JSON",
            getDefaultValue: () => false);

        var urlArgument = new Argument<string>(
            name: "url",
            description: "URL to scan.",
            getDefaultValue: () => null);
        
        var rootCommand = new RootCommand("Command-line version of the scanner used by https://headers.security/.");
        rootCommand.AddGlobalOption(fileOption);
        rootCommand.AddGlobalOption(jsonOption);

        var scanCommand = new Command("scan", "Perform a security header scan for the given URI.");
        scanCommand.AddArgument(urlArgument);
        scanCommand.SetHandler(async (url, asJson, file) => await HandleScanCommand(url, asJson, file), urlArgument, jsonOption, fileOption);
        
        rootCommand.AddCommand(scanCommand);
        
        return await rootCommand.InvokeAsync(args);
    }

    private static async Task HandleScanCommand(string url, bool asJson, FileSystemInfo file)
    {
        if (file == null && string.IsNullOrWhiteSpace(url))
        {
            await Console.Error.WriteLineAsync("Must pass URL in argument or use file list option.");
            Environment.Exit(-1);
        }
        else if (file != null && !string.IsNullOrWhiteSpace(url))
        {
            await Console.Error.WriteLineAsync("Can't both pass URL in argument and use file list.");
            Environment.Exit(-1);
        }

        List<Uri> uris = null;

        try
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                uris = [new Uri(url)];
            }
            else
            {
                uris = ReadFile(file!)
                    .Select(line => new Uri(line))
                    .ToList();
            }
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync(e.Message);
            Environment.Exit(-1);
        }
        
        var services = new ServiceCollection();
        services.AddHttpClient("Scanner", HttpClientHelper.ConfigureClient);
        services.AddCachedContent(useBackgroundService: false);
        
        var serviceProvider = services.BuildServiceProvider();

        var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

        var conf = new WorkerConfiguration
        {
            IPv6Enabled = false
        };

        var crawler = new Crawler(conf, httpClientFactory);

        var securityConceptTypes = SecurityConceptResolver.GetSecurityConcepts();

        var handlers = securityConceptTypes
            .Select(Activator.CreateInstance)
            .Cast<ISecurityConcept>()
            .ToList();

        var securityEngine = new SecurityEngine(handlers);
        var worker = new Worker(crawler, securityEngine);
        
        var configuration = new CrawlerConfiguration
        {
            FollowRedirects = true
        };
        
        var results = await worker.PerformScan(configuration, uris);
        
        Console.WriteLine(results.Count);
    }

    private static List<string> ReadFile(FileSystemInfo file)
        => File.ReadLines(file.FullName).ToList();
}