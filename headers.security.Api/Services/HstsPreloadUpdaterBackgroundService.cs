using System.Text.Json;
using headers.security.Scanner.Hsts;
using Microsoft.Extensions.Caching.Memory;

namespace headers.security.Api.Services;

public class HstsPreloadUpdaterBackgroundService(
    ILogger<HstsPreloadUpdaterBackgroundService> logger,
    IServiceProvider services,
    IHostEnvironment environment,
    IMemoryCache cache)
    : BackgroundService
{

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("{Service} is running", nameof(HstsPreloadUpdaterBackgroundService));
        await BackgroundProcessing(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("{Service} is stopping", nameof(HstsPreloadUpdaterBackgroundService));
        await base.StopAsync(cancellationToken);
    }

    private const string StateFilename = "hstspreload-state.json";

    private async Task BackgroundProcessing(CancellationToken cancellationToken)
    {
        if (environment.IsDevelopment() && !cache.TryGetValue(CachingHstsPreloadRepository.CacheKey, out _) && File.Exists(StateFilename))
        {
            logger.LogInformation("Restoring state from file");
            
            await using var file = File.OpenRead(StateFilename);
            var tree = await JsonSerializer.DeserializeAsync<PreloadPolicyNode>(file, cancellationToken: cancellationToken);

            using var scope = services.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IHstsPreloadRepository>();
            
            repository.RestoreState(tree);
        }
        
        while (!cancellationToken.IsCancellationRequested)
        {
            var delay = GetNextRun();
            
            logger.LogInformation("Time to next update: {Delay}", delay);
            await Task.Delay(delay, cancellationToken);

            using var scope = services.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IHstsPreloadRepository>();
            
            try
            {
                var tree = await repository.UpdatePreloadCache();
                if (environment.IsDevelopment() && tree != null)
                {
                    logger.LogInformation("Saving state to file");
                    
                    await using var file = File.OpenWrite(StateFilename);
                    await JsonSerializer.SerializeAsync(file, tree, cancellationToken: cancellationToken);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to update preload cache on schedule");
            }
        }
    }

    private TimeSpan GetNextRun()
    {
        var expirationDate = cache.Get<DateTime?>(CachingHstsPreloadRepository.ExpiryCacheKey) ?? DateTime.UtcNow;
        
        var timeUntil = expirationDate - DateTime.UtcNow;
        if (timeUntil < TimeSpan.FromMinutes(5))
        {
            return TimeSpan.FromSeconds(GetRandom(30));
        }

        var minutesLeft = timeUntil.TotalMinutes;
        if (minutesLeft > 60)
        {
            return timeUntil - TimeSpan.FromMinutes(GetRandom());
        }

        var minutes = TimeSpan.FromMinutes(Math.Max(1, GetRandom(minutesLeft)));
        var seconds = TimeSpan.FromSeconds(GetRandom());

        return minutes.Add(seconds);
    }

    private static int GetRandom(double maxValue = 60) => Random.Shared.Next(1, (int) maxValue);
}