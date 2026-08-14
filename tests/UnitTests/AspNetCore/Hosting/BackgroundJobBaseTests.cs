using System.Diagnostics;
using AuroraScienceHub.Framework.AspNetCore.Hosting;
using AuroraScienceHub.Framework.Composition;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;

namespace AuroraScienceHub.Framework.UnitTests.AspNetCore.Hosting;

/// <summary>
/// Tests for <see cref="BackgroundJobBase"/>
/// </summary>
public sealed class BackgroundJobBaseTests
{
    [Fact(DisplayName = "Periodic job runs once on application startup, not after a full period")]
    public async Task ExecuteAsync_WhenPeriodicJob_ExecutesImmediatelyOnStartup()
    {
        // Arrange
        var job = CreateJob(new JobOptions
        {
            Name = nameof(TestJob),
            Period = TimeSpan.FromHours(1),
            Enabled = true,
        });
        using var cts = new CancellationTokenSource();

        // Act
        await job.StartAsync(cts.Token);
        await job.WaitUntilExecutedAsync(1, TimeSpan.FromSeconds(5), cts.Token);

        // Assert
        job.ExecutionCount.ShouldBe(1);

        // Cleanup
        await StopJobAsync(job);
    }

    [Fact(DisplayName = "Periodic job keeps executing on every period after the first run")]
    public async Task ExecuteAsync_WhenPeriodicJob_ExecutesOnEveryPeriod()
    {
        // Arrange
        var job = CreateJob(new JobOptions
        {
            Name = nameof(TestJob),
            Period = TimeSpan.FromMilliseconds(50),
            Enabled = true,
        });
        using var cts = new CancellationTokenSource();

        // Act
        await job.StartAsync(cts.Token);
        await job.WaitUntilExecutedAsync(3, TimeSpan.FromSeconds(5), cts.Token);

        // Assert
        job.ExecutionCount.ShouldBeGreaterThanOrEqualTo(3);

        // Cleanup
        await StopJobAsync(job);
    }

    [Fact(DisplayName = "Job without a period runs once and does not repeat")]
    public async Task ExecuteAsync_WhenNoPeriod_ExecutesOnceAndDoesNotRepeat()
    {
        // Arrange
        var job = CreateJob(new JobOptions
        {
            Name = nameof(TestJob),
            Period = null,
            Enabled = true,
        });
        using var cts = new CancellationTokenSource();

        // Act
        await job.StartAsync(cts.Token);
        await job.WaitUntilExecutedAsync(1, TimeSpan.FromSeconds(5), cts.Token);
        await Task.Delay(150, cts.Token);

        // Assert
        job.ExecutionCount.ShouldBe(1);

        // Cleanup
        await StopJobAsync(job);
    }

    [Fact(DisplayName = "Disabled job does not execute")]
    public async Task ExecuteAsync_WhenDisabled_DoesNotExecute()
    {
        // Arrange
        var job = CreateJob(new JobOptions
        {
            Name = nameof(TestJob),
            Period = TimeSpan.FromMilliseconds(50),
            Enabled = false,
        });
        using var cts = new CancellationTokenSource();

        // Act
        await job.StartAsync(cts.Token);
        await Task.Delay(150, cts.Token);

        // Assert
        job.ExecutionCount.ShouldBe(0);

        // Cleanup
        await StopJobAsync(job);
    }

    private static TestJob CreateJob(JobOptions jobOptions)
        => new(
            CreateLifetime(),
            NullLogger<BackgroundJobBase>.Instance,
            Options.Create<ApplicationModuleOptionsBase>(new DefaultApplicationModuleOptions
            {
                ScheduledJobs = [jobOptions],
            }));

    private static IHostApplicationLifetime CreateLifetime()
    {
        var lifetimeMock = new Mock<IHostApplicationLifetime>();
        // A token that is already canceled but not backed by a CancellationTokenSource,
        // so registering on it cannot throw ObjectDisposedException.
        lifetimeMock.Setup(l => l.ApplicationStarted).Returns(new CancellationToken(canceled: true));
        lifetimeMock.Setup(l => l.ApplicationStopping).Returns(CancellationToken.None);
        lifetimeMock.Setup(l => l.ApplicationStopped).Returns(CancellationToken.None);
        return lifetimeMock.Object;
    }

    private static async Task StopJobAsync(BackgroundJobBase job)
    {
        try
        {
            await job.StopAsync(CancellationToken.None);
        }
        catch (OperationCanceledException)
        {
            // Expected when the background service is stopped while waiting for a period tick.
        }
    }

    private sealed class TestJob : BackgroundJobBase
    {
        private int _executionCount;

        public TestJob(IHostApplicationLifetime lifetime, ILogger<TestJob> logger,
            IOptions<ApplicationModuleOptionsBase> moduleOptions)
            : base(lifetime, logger, moduleOptions)
        {
        }

        public int ExecutionCount => Volatile.Read(ref _executionCount);

        public override Task ExecuteOnceAsync(CancellationToken stoppingToken)
        {
            Interlocked.Increment(ref _executionCount);
            return Task.CompletedTask;
        }

        public async Task WaitUntilExecutedAsync(int count, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            while (ExecutionCount < count)
            {
                if (stopwatch.Elapsed > timeout)
                {
                    throw new TimeoutException($"Job executed {ExecutionCount} time(s), expected at least {count}.");
                }

                await Task.Delay(10, cancellationToken);
            }
        }

        protected override Task RandomDelayAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
