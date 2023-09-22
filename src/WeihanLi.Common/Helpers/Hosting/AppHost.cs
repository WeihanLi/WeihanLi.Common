// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers.Hosting;

public interface IAppHost
{
    IConfiguration Configuration { get; }
    IServiceProvider Services { get; }
    Task RunAsync(CancellationToken cancellationToken = default);
}

public sealed class AppHost : IAppHost
{
    private const string
        AppHostStartingMessage = "AppHost starting",
        AppHostStartedMessage = "AppHost started. Press Ctrl+C to shut down",
        AppHostStoppingMessage = "AppHost stopping",
        AppHostStoppedMessage = "AppHost stopped"
        ;

    private readonly ILogger _logger;
    private readonly AppHostOptions _appHostOptions;

    private readonly IHostedService[] _hostedServices;
    private readonly IHostedLifecycleService[] _hostedLifecycleServices;

    public AppHost(IServiceProvider services, IConfiguration configuration)
    {
        Services = services;
        Configuration = configuration;
        _logger = services.GetRequiredService<ILogger<AppHost>>();
        _appHostOptions = services.GetRequiredService<IOptions<AppHostOptions>>().Value;

        _hostedServices = services.GetServices<IHostedService>().ToArray();
        _hostedLifecycleServices = _hostedServices.Select(x => x as IHostedLifecycleService)
            .WhereNotNull().ToArray();
    }

    public IConfiguration Configuration { get; }
    public ILogger Logger => _logger;
    public IServiceProvider Services { get; }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        Debug.WriteLine(AppHostStartingMessage);
        _logger.LogInformation(AppHostStartingMessage);
        using var hostStopTokenSource = CancellationTokenSource.CreateLinkedTokenSource(InvokeHelper.GetExitToken(), cancellationToken);
#if NET6_0_OR_GREATER
        var waitForStopTask = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        hostStopTokenSource.Token.Register(() => waitForStopTask.TrySetResult());
#else
        var waitForStopTask = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        hostStopTokenSource.Token.Register(() => waitForStopTask.TrySetResult(null));
#endif
        var exceptions = new List<Exception>();

        var startTimeoutCts = new CancellationTokenSource(_appHostOptions.StartupTimeout);
        var hostStartCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, InvokeHelper.GetExitToken(), startTimeoutCts.Token);
        var hostStartCancellationToken = hostStartCancellationTokenSource.Token;
        await ForeachService(_hostedLifecycleServices, hostStartCancellationToken, _appHostOptions.ServicesStartConcurrently,
            !_appHostOptions.ServicesStartConcurrently, exceptions, async (service, cancelToken) =>
            {
                await service.StartingAsync(cancelToken);
            }).ConfigureAwait(false);
        LogAndRethrow();
        await ForeachService(_hostedServices, hostStartCancellationToken, _appHostOptions.ServicesStartConcurrently,
                            !_appHostOptions.ServicesStartConcurrently, exceptions, async (service, cancelToken) =>
                            {
                                await service.StartAsync(cancelToken);
                            }).ConfigureAwait(false);
        LogAndRethrow();
        await ForeachService(_hostedLifecycleServices, hostStartCancellationToken, _appHostOptions.ServicesStartConcurrently,
                    !_appHostOptions.ServicesStartConcurrently, exceptions, async (service, cancelToken) =>
                    {
                        await service.StartedAsync(cancelToken);
                    }).ConfigureAwait(false);
        LogAndRethrow();
        startTimeoutCts.Dispose();
        Debug.WriteLine(AppHostStartedMessage);
        _logger.LogInformation(AppHostStartedMessage);

        await waitForStopTask.Task.ConfigureAwait(false);
        Debug.WriteLine(AppHostStoppingMessage);
        _logger.LogInformation(AppHostStoppingMessage);
        // reverse to keep first startup last stop when not in concurrent
        Array.Reverse(_hostedServices);
        Array.Reverse(_hostedLifecycleServices);
        var stopTimeoutCts = new CancellationTokenSource(_appHostOptions.ShutdownTimeout);
        var hostStopCancellationToken = stopTimeoutCts.Token;
        await ForeachService(_hostedLifecycleServices, hostStopCancellationToken, _appHostOptions.ServicesStopConcurrently,
            false, exceptions, async (service, cancelToken) =>
            {
                await service.StoppingAsync(cancelToken);
            }).ConfigureAwait(false);
        await ForeachService(_hostedServices, hostStopCancellationToken, _appHostOptions.ServicesStopConcurrently,
            false, exceptions, async (service, cancelToken) =>
            {
                await service.StopAsync(cancelToken);
            }).ConfigureAwait(false);
        await ForeachService(_hostedLifecycleServices, hostStopCancellationToken, _appHostOptions.ServicesStopConcurrently,
            false, exceptions, async (service, cancelToken) =>
            {
                await service.StoppedAsync(cancelToken);
            }).ConfigureAwait(false);

        Debug.WriteLine(AppHostStoppedMessage);
        _logger.LogInformation(AppHostStoppedMessage);

        // Log and abort if there are exceptions.
        void LogAndRethrow()
        {
            if (exceptions is not { Count: > 0 }) return;

            if (exceptions.Count == 1)
            {
                // Rethrow if it's a single error
                var singleException = exceptions[0];
                _logger.LogCritical(singleException, "AppHost Startup exception");
                ExceptionDispatchInfo.Capture(singleException).Throw();
            }
            else
            {
                var ex = new AggregateException("One or more hosted services failed to start.", exceptions);
                _logger.LogCritical(ex, "AppHost Startup exception");
                throw ex;
            }
        }
    }

    public static AppHostBuilder CreateBuilder(AppHostBuilderSettings? settings = null)
    {
        return new AppHostBuilder(settings);
    }

    private static async Task ForeachService<T>(
        T[] services,
        CancellationToken token,
        bool concurrent,
        bool abortOnFirstException,
        List<Exception> exceptions,
        Func<T, CancellationToken, Task> operation)
    {
        if (services.IsNullOrEmpty()) return;

        if (concurrent)
        {
            // The beginning synchronous portions of the implementations are run serially in registration order for
            // performance since it is common to return Task.Completed as a noop.
            // Any subsequent asynchronous portions are grouped together and run concurrently.
            List<Task>? tasks = null;

            foreach (var service in services)
            {
                Task task;
                try
                {
                    task = operation(service, token);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex); // Log exception from sync method.
                    continue;
                }

                if (task.IsCompleted)
                {
                    if (task.Exception is not null)
                    {
                        exceptions.AddRange(task.Exception.InnerExceptions); // Log exception from async method.
                    }
                }
                else
                {
                    // The task encountered an await; add it to a list to run concurrently.
                    tasks ??= new();
                    tasks.Add(Task.Run(() => task, token));
                }
            }

            if (tasks is not null)
            {
                var groupedTasks = Task.WhenAll(tasks);

                try
                {
                    await groupedTasks.ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    exceptions.AddRange(groupedTasks.Exception?.InnerExceptions ?? new[] { ex }.AsEnumerable());
                }
            }
        }
        else
        {
            foreach (var service in services)
            {
                try
                {
                    await operation(service, token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                    if (abortOnFirstException)
                    {
                        return;
                    }
                }
            }
        }
    }
}

