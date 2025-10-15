using AuroraScienceHub.Framework.Composition;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Framework.AspNetCore.Hosting;

/// <summary>
/// Base class for periodically executed background jobs
/// </summary>
public abstract class BackgroundJobBase : BackgroundService
{
    private PeriodicTimer? _timer;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly JobOptions _options;

    protected readonly ILogger Logger;

    private string JobName => GetType().Name;

    protected BackgroundJobBase(
        IHostApplicationLifetime lifetime,
        ILogger<BackgroundJobBase> logger,
        IOptions<ApplicationModuleOptionsBase> moduleOptions)
    {
        _lifetime = lifetime;
        Logger = logger;
        _options = moduleOptions.Value.GetJobOptions(JobName);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!await WaitForAppStartupAsync(_lifetime, stoppingToken).ConfigureAwait(false))
        {
            return;
        }

        if (!_options.Enabled)
        {
            Logger.LogInformation("Background job {JobName} is disabled", JobName);
            return;
        }

        await RandomDelayAsync(stoppingToken).ConfigureAwait(false);

        _timer = new PeriodicTimer(_options.Period ?? Timeout.InfiniteTimeSpan);

        while (await _timer.WaitForNextTickAsync(stoppingToken)
               && !stoppingToken.IsCancellationRequested)
        {
            try
            {
                Logger.LogInformation("Starting background job {JobName} (period: '{Period}')..", JobName, _options.Period);

                await ExecuteOnceAsync(stoppingToken).ConfigureAwait(false);

                Logger.LogInformation("Background job {JobName} completed successfully", JobName);
            }
            catch (Exception exception)
            {
                Logger.LogInformation(exception, "Background {JobName} job failed", JobName);
            }

            // If the periodicity was set to InfiniteTimeSpan, then we stop the background service after the first task execution
            if (_timer.Period == Timeout.InfiniteTimeSpan)
            {
                return;
            }
        }
    }

    /// <summary>
    /// Execute a task in the background
    /// </summary>
    public abstract Task ExecuteOnceAsync(CancellationToken stoppingToken);

    private static async Task<bool> WaitForAppStartupAsync(IHostApplicationLifetime lifetime,
        CancellationToken stoppingToken)
    {
        var startedSource = new TaskCompletionSource();
        var cancelledSource = new TaskCompletionSource();
        await using var reg1 = lifetime.ApplicationStarted.Register(() => startedSource.SetResult());
        await using var reg2 = stoppingToken.Register(() => cancelledSource.SetResult());

        var completedTask = await Task.WhenAny(startedSource.Task, cancelledSource.Task).ConfigureAwait(false);

        return completedTask == startedSource.Task;
    }

    public override void Dispose()
        => _timer?.Dispose();

    private async Task RandomDelayAsync(CancellationToken cancellationToken)
    {
        const int minDelaySec = 5;
        const int maxDelaySec = 30;

        var delay = Random.Shared.Next(minDelaySec, maxDelaySec);
        Logger.LogDebug("Waiting {Delay} sec. before starting the background service", delay);
        await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken).ConfigureAwait(false);
    }
}
