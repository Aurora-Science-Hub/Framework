using System.Diagnostics;
using AuroraScienceHub.Framework.Composition;
using AuroraScienceHub.Framework.Utilities.System;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuroraScienceHub.Framework.AspNetCore.Hosting;

/// <summary>
/// A background job that runs every specified period.
/// </summary>
public abstract class ScheduledBackgroundJob : BackgroundJobBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<IBaseRequest> _commands;

    private static readonly ActivitySource s_activitySource =
        new(typeof(ScheduledBackgroundJob).Assembly.GetName().Name.Required(),
            typeof(ScheduledBackgroundJob).Assembly.GetName().Version.Required().ToString());

    protected ScheduledBackgroundJob(
        IHostApplicationLifetime lifetime,
        IServiceProvider serviceProvider,
        ILogger<ScheduledBackgroundJob> logger,
        IOptions<ApplicationModuleOptionsBase> moduleOptions,
        IBaseRequest command)
        : base(lifetime, logger, moduleOptions)
    {
        _serviceProvider = serviceProvider;
        _commands = [command];
    }

    protected ScheduledBackgroundJob(
        IHostApplicationLifetime lifetime,
        IServiceProvider serviceProvider,
        ILogger<ScheduledBackgroundJob> logger,
        IOptions<ApplicationModuleOptionsBase> moduleOptions,
        params IEnumerable<IBaseRequest> commands)
        : base(lifetime, logger, moduleOptions)
    {
        _serviceProvider = serviceProvider;
        _commands = commands;
    }

    static ScheduledBackgroundJob()
    {
        var listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == s_activitySource.Name,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded
        };
        ActivitySource.AddActivityListener(listener);
    }

    public override async Task ExecuteOnceAsync(CancellationToken stoppingToken)
    {
        using var activity = s_activitySource.StartActivity(_commands.GetType().Name);
        Activity.Current = activity;

        await using var scope = _serviceProvider.CreateAsyncScope();
        var bus = scope.ServiceProvider.GetRequiredService<IMediator>();

        foreach (var command in _commands)
        {
            var stopwatch = Stopwatch.StartNew();

            await bus.Send(command, stoppingToken).ConfigureAwait(false);

            stopwatch.Stop();
            Logger.LogInformation("Command {Command} has been executed for {Elapsed}",
                command.GetType().Name,
                stopwatch.Elapsed.ToString("hh\\:mm\\:ss\\.fff"));
        }
    }
}
