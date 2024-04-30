using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace headers.security.CachedContent.Services;

// TODO: Future, add an enum RunMode here and make sure CLI is initialized correctly before running scan
public class CachedContentBackgroundUpdateService(
    ILogger<CachedContentBackgroundUpdateService> logger,
    IServiceProvider services,
    IHostEnvironment environment,
    IMemoryCache cache)
    : BackgroundService
{
    private const string StateDirectory = ".cachedContentState";

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("{Service} is running", nameof(CachedContentBackgroundUpdateService));
        
        if (environment.IsDevelopment()) 
            await RestoreStates(cancellationToken);
        
        await MaintainStates(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("{Service} is stopping", nameof(CachedContentBackgroundUpdateService));
        await base.StopAsync(cancellationToken);
    }

    private async Task RestoreStates(CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        var tasks = scope.ServiceProvider.GetServices<ICachedContentRepository>()
            .Select(repo => RestoreState(repo, cancellationToken));

        await Task.WhenAll(tasks);
    }

    private async Task RestoreState(ICachedContentRepository repository, CancellationToken cancellationToken)
    {
        logger.LogInformation("Restoring {TypeName} state from file", repository.Type.Name);

        if (!cache.TryGetValue(repository.CacheKey, out _) && File.Exists($"{StateDirectory}/{repository.StateFilename}.json"))
        {
            try
            {
                await using var stream = File.OpenRead($"{StateDirectory}/{repository.StateFilename}.json");
                var state = await JsonSerializer.DeserializeAsync(stream, repository.Type,
                    cancellationToken: cancellationToken);

                repository.RestoreState(state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    private async Task MaintainStates(CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        var tasks = scope.ServiceProvider.GetServices<ICachedContentRepository>()
            .Select(repo => MaintainState(repo, cancellationToken));

        await Task.WhenAll(tasks);
    }

    private async Task MaintainState(ICachedContentRepository repository, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var delay = GetNextRun(repository);
            
            logger.LogInformation("Time to next {TypeName} update: {Delay}", repository.Type.Name, delay);
            await Task.Delay(delay, cancellationToken);
            
            var state = await repository.UpdateCache();
            if (environment.IsDevelopment() && state != null)
            {
                await PersistState(repository, state, cancellationToken);
            }
        }
    }

    private async Task PersistState(ICachedContentRepository repository, object state, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(StateDirectory))
        {
            Directory.CreateDirectory(StateDirectory);
        }
        
        logger.LogInformation("Saving {TypeName} state to file", repository.Type.Name);
        
        await using var file = File.OpenWrite($"{StateDirectory}/{repository.StateFilename}.json");
        await JsonSerializer.SerializeAsync(file, state, repository.Type, cancellationToken: cancellationToken);
    }

    private TimeSpan GetNextRun(ICachedContentRepository repository)
    {
        var expirationDate = cache.Get<DateTime?>(repository.ExpiryCacheKey) ?? DateTime.UtcNow;
        
        var timeUntil = expirationDate - DateTime.UtcNow;
        if (timeUntil < TimeSpan.FromMinutes(5))
        {
            return TimeSpan.FromSeconds(GetRandom(3));
        }

        var minutesLeft = timeUntil.TotalMinutes;
        if (minutesLeft > 60)
        {
            return timeUntil - TimeSpan.FromMinutes(GetRandom());
        }

        var minutes = TimeSpan.FromMinutes(GetRandom(minutesLeft));
        var seconds = TimeSpan.FromSeconds(GetRandom());

        return minutes.Add(seconds);
    }

    private static int GetRandom(double maxValue = 60) => Random.Shared.Next(1, Math.Max(1, (int) maxValue));
}