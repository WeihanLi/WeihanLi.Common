// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Diagnostics;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers;

public interface IBuildProcessBuilder
{
    IBuildProcessBuilder WithTask(string name, Action<IBuildTaskBuilder> buildTaskConfigure);

    IBuildProcessBuilder WithSetup(Func<Task> setupFunc);

    IBuildProcessBuilder WithCleanup(Func<Task> cleanupFunc);

    IBuildProcessBuilder WithCancelled(Func<Task> cancelledFunc);

    IBuildProcessBuilder WithTaskExecuting(Func<IBuildTaskDescriptor, Task> taskExecutingAction);

    IBuildProcessBuilder WithTaskExecuted(Func<IBuildTaskDescriptor, Task> taskExecutedAction);
}

public interface IBuildTaskDescriptor
{
    string Name { get; }
    string Description { get; }
}

public interface IBuildTaskBuilder
{
    IBuildTaskBuilder WithDescription(string? description);
    IBuildTaskBuilder WithDependency(string dependencyTaskName);
    IBuildTaskBuilder WithExecution(Func<CancellationToken, Task> execution);
}

public static class BuildProcessExtensions
{
    extension(IBuildProcessBuilder builder)
    {
        public IBuildProcessBuilder WithSetup(Action setupAction) => builder.WithSetup(setupAction.WrapTask());
        public IBuildProcessBuilder WithCleanup(Action cleanupAction) => builder.WithCleanup(cleanupAction.WrapTask());
        public IBuildProcessBuilder WithCancelled(Action cancelledAction) => builder.WithCancelled(cancelledAction.WrapTask());
        public IBuildProcessBuilder WithTaskExecuting(Action<IBuildTaskDescriptor> taskExecutingAction) 
            => builder.WithTaskExecuting(taskExecutingAction.WrapTask());
        public IBuildProcessBuilder WithTaskExecuted(Action<IBuildTaskDescriptor> taskExecutedAction) 
            => builder.WithTaskExecuted(taskExecutedAction.WrapTask());
        public IBuildProcessBuilder WithTaskExecution(string name, Action taskExecution)
            => builder.WithTask(name, taskBuilder => taskBuilder.WithExecution(taskExecution));
        public IBuildProcessBuilder WithTaskExecution(string name, Func<Task> taskExecution)
            => builder.WithTask(name, taskBuilder => taskBuilder.WithExecution(taskExecution));
        public IBuildProcessBuilder WithTaskExecution(string name, Func<CancellationToken, Task> taskExecution)
            => builder.WithTask(name, taskBuilder => taskBuilder.WithExecution(taskExecution));
    }

    extension(IBuildTaskBuilder builder)
    {
        public IBuildTaskBuilder WithExecution(Action executionAction) => builder.WithExecution(executionAction.WrapTask());
        public IBuildTaskBuilder WithExecution(Func<Task> executionFunc) => builder.WithExecution(executionFunc.WrapCancellation());
    }

    extension(BuildProcess)
    {
        public static IBuildProcessBuilder CreateBuilder() => new BuildProcessBuilder();
    }

    extension(DotNetBuildProcessOptions options)
    {
        public DotNetBuildProcessOptions WithTaskExecution(string name, Action taskExecution)
            => options.WithTaskExecution(name, taskExecution.WrapTask());
        public DotNetBuildProcessOptions WithTaskExecution(string name, Func<Task> taskExecution)
            => options.WithTaskExecution(name, taskExecution.WrapCancellation());
    }
}

public sealed class BuildProcess(IReadOnlyCollection<BuildTask> tasks,
    Func<Task>? setup = null, Func<Task>? cleanup = null, Func<Task>? cancelled = null,
    Func<IBuildTaskDescriptor, Task>? taskExecuting = null, Func<IBuildTaskDescriptor, Task>? taskExecuted = null)
{
    private readonly IReadOnlyCollection<BuildTask> _tasks = tasks;
    private readonly Func<Task>? _setup = setup, _cleanup = cleanup, _cancelled = cancelled;
    private readonly Func<IBuildTaskDescriptor, Task>? _taskExecuting = taskExecuting, _taskExecuted = taskExecuted;

    public async Task ExecuteAsync(string target, CancellationToken cancellationToken = default)
    {
        var task = _tasks.FirstOrDefault(x => x.Name == target);
        if (task is null)
            throw new InvalidOperationException("Invalid target to execute");

        try
        {
            if (_setup != null)
                await _setup.Invoke();

            await ExecuteTask(task, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            if (_cancelled != null)
                await _cancelled.Invoke();
        }
        finally
        {
            if (_cleanup != null)
                await _cleanup.Invoke();
        }
    }

    private async Task ExecuteTask(BuildTask task, CancellationToken cancellationToken)
    {
        foreach (var dependencyTask in task.Dependencies)
        {
            await ExecuteTask(dependencyTask, cancellationToken);
        }

        if (_taskExecuting != null)
            await _taskExecuting.Invoke(task);
        await task.ExecuteAsync(cancellationToken);
        if (_taskExecuted != null)
            await _taskExecuted.Invoke(task);
    }
}

public sealed class BuildTask(string name, string? description, Func<CancellationToken, Task>? execution = null) 
    : IBuildTaskDescriptor
{
    private IReadOnlyCollection<BuildTask> _dependencies = [];

    public string Name => name;
    public string Description => description ?? name;
    public IReadOnlyCollection<BuildTask> Dependencies => _dependencies;

    internal void SetDependencies(IReadOnlyCollection<BuildTask> dependencies)
    {
        _dependencies = dependencies;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken) =>
        execution?.Invoke(cancellationToken) ?? Task.CompletedTask;
}

internal sealed class BuildTaskBuilder(string name) : IBuildTaskBuilder
{
    private readonly string _name = name;

    private string? _description;
    private Func<CancellationToken, Task>? _execution;
    private readonly List<string> _dependencies = [];

    public IBuildTaskBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public IBuildTaskBuilder WithExecution(Func<CancellationToken, Task> execution)
    {
        _execution = execution;
        return this;
    }

    public IBuildTaskBuilder WithDependency(string dependencyTaskName)
    {
        if (string.IsNullOrWhiteSpace(dependencyTaskName))
            throw new ArgumentException(@"Dependency task name could not be null or whitespace", nameof(dependencyTaskName));
        
        _dependencies.Add(dependencyTaskName);
        return this;
    }

    internal IReadOnlyCollection<string> DependencyNames => _dependencies;

    internal BuildTask Build()
    {
        return new BuildTask(_name, _description, _execution);
    }
}

internal sealed class BuildProcessBuilder : IBuildProcessBuilder
{
    private readonly Dictionary<string, Action<BuildTaskBuilder>> _taskBuilders = new(StringComparer.Ordinal);
    private Func<Task>? _setup, _cleanup, _cancelled;
    private Func<IBuildTaskDescriptor, Task>? _taskExecuting, _taskExecuted;

    public IBuildProcessBuilder WithTask(string name, Action<IBuildTaskBuilder> buildTaskConfigure)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(@"Task name could not be null or whitespace", nameof(name));

        if (_taskBuilders.TryGetValue(name, out var taskBuilder))
        {
         
            _taskBuilders[name] = taskBuilder + buildTaskConfigure;   
        }
        else
        {
            _taskBuilders[name] = buildTaskConfigure;            
        }

        return this;
    }

    public IBuildProcessBuilder WithSetup(Func<Task> setupFunc)
    {
        _setup = setupFunc;
        return this;
    }

    public IBuildProcessBuilder WithCleanup(Func<Task> cleanupFunc)
    {
        _cleanup = cleanupFunc;
        return this;
    }

    public IBuildProcessBuilder WithCancelled(Func<Task> cancelledFunc)
    {
        _cancelled = cancelledFunc;
        return this;
    }

    public IBuildProcessBuilder WithTaskExecuting(Func<IBuildTaskDescriptor, Task> taskExecutingAction)
    {
        _taskExecuting = taskExecutingAction;
        return this;
    }

    public IBuildProcessBuilder WithTaskExecuted(Func<IBuildTaskDescriptor, Task> taskExecutedAction)
    {
        _taskExecuted = taskExecutedAction;
        return this;
    }

    public BuildProcess Build()
    {
        if (_taskBuilders.Count == 0)
        {
            throw new InvalidOperationException("No tasks configured");
        }

        var tasksDictionary = new Dictionary<string, (BuildTaskBuilder TaskBuilder, BuildTask Task)>(StringComparer.Ordinal);
        foreach (var taskName in _taskBuilders.Keys)
        {
            var taskBuilder = new BuildTaskBuilder(taskName);
            _taskBuilders[taskName].Invoke(taskBuilder);
            tasksDictionary[taskName] = (taskBuilder, taskBuilder.Build());
        }

        foreach (var taskName in _taskBuilders.Keys)
        {
            var builder = tasksDictionary[taskName];
            var dependencies = builder.TaskBuilder.DependencyNames.Select(name =>
            {
                if (!tasksDictionary.TryGetValue(name, out var dependency))
                {
                    throw new InvalidOperationException($"No task found with name {name}");
                }

                return dependency.Task;
            }).ToArray();

            tasksDictionary[taskName].Task.SetDependencies(dependencies);
        }

        var tasks = tasksDictionary.Values.Select(t => t.Task).ToArray();
        return new BuildProcess(tasks, _setup, _cleanup, _cancelled, _taskExecuting, _taskExecuted);
    }
}

public sealed class DotNetBuildProcessOptions
{
    public string? SolutionPath { get; set; }
    public string[]? SrcProjects { get; set; }
    public string[]? TestProjects { get; set; }
    public string[]? RunFileSampleFolders { get; set; }
    public Func<string?> FallbackNuGetApiKeyFunc { get; set; } = () => EnvHelper.Val("NuGet__ApiKey");
    public Func<string?> FallbackNuGetSourceFunc { get; set; } = () => EnvHelper.Val("NuGet__Source");
    public string ArtifactsPath { get; set; } = "./artifacts/dist";
    public Func<string?> BranchFunc { get; set; } = () => EnvHelper.Val("BUILD_SOURCEBRANCHNAME", EnvHelper.Val("GITHUB_REF_NAME"));
    public bool AllowLocalPush { get; set; }

    internal readonly Dictionary<string, Action<IBuildTaskBuilder>> TaskOverride = new();
    
    public DotNetBuildProcessOptions WithTaskConfigure(string name, Action<IBuildTaskBuilder> taskConfigure)
    {
        TaskOverride[name] = taskConfigure;
        return this;
    }
    
    public DotNetBuildProcessOptions WithTaskExecution(string name, Func<CancellationToken, Task> taskExecution)
    {
        TaskOverride[name] = x => x.WithExecution(taskExecution);
        return this;
    }
}

public sealed class DotNetPackageBuildProcess
{
    private readonly BuildProcess _buildProcess;
    private string? _apiKey, _source, _branch;
    private bool _stable, _noPush;

    private DotNetPackageBuildProcess(DotNetBuildProcessOptions options)
    {
        _branch = options.BranchFunc();
        var builder = BuildProcess.CreateBuilder()
            .WithSetup(() =>
            {
                // cleanup artifacts
                if (Directory.Exists(options.ArtifactsPath))
                    Directory.Delete(options.ArtifactsPath, true);

                // args
                Console.WriteLine(@"Executing command line:");
                ConsoleHelper.WriteLineWithColor($@"  {Environment.CommandLine}", ConsoleColor.DarkGreen);
                Console.WriteLine($@"Branch: {_branch}, stable: {_stable}");
            })
            .WithTaskExecuting(task => ConsoleHelper.WriteLineWithColor($@"===== Task {task.Name} {task.Description} executing ======", ConsoleColor.DarkCyan))
            .WithTaskExecuted(task => ConsoleHelper.WriteLineWithColor($@"===== Task {task.Name} {task.Description} executed ======", ConsoleColor.DarkGreen))
            .WithTask("build", b =>
            {
                b.WithDescription("dotnet build")
                  .WithExecution(() =>
                  {
                      Console.WriteLine($@"Build solution {options.SolutionPath}");
                      CommandExecutor.ExecuteCommandAndOutput($"dotnet build {options.SolutionPath}")
                          .EnsureSuccessExitCode();
                      
                      if (options.RunFileSampleFolders is not { Length: > 0 })
                          return;

                      Parallel.ForEach(options.RunFileSampleFolders, folder =>
                      {
                          foreach (var file in Directory.GetFiles(folder, "*.cs"))
                          {
                              CommandExecutor.ExecuteAndOutput($"dotnet build {Path.GetFullPath(file)}");
                          }
                      });
                  });
            })
            .WithTask("test", b =>
            {
                b.WithDescription("dotnet test")
                    .WithDependency("build")
                    .WithExecution(() =>
                    {
                        var disableGitHubReport = EnvHelper.BooleanVal("DISABLE_GITHUB_ACTIONS_TEST_LOGGER");
                        var enableGitHubReport =
                            string.Equals(EnvHelper.Val("GITHUB_ACTIONS"), "true", StringComparison.OrdinalIgnoreCase) &&
                            !disableGitHubReport;
                        var reportArguments = enableGitHubReport ? " --report-github" : string.Empty;

                        foreach (var project in options.TestProjects ?? [])
                        {
                            CommandExecutor.ExecuteCommandAndOutput(
                                $"dotnet run --project {project}{reportArguments}"
                                ).EnsureSuccessExitCode();
                        }
                    })
                    ;
            })
            .WithTask("pack", b => b.WithDescription("dotnet pack")
                .WithDependency("test")
                .WithExecution(() =>
                {
                    if (options.SrcProjects is not { Length: > 0 })
                        return;
                    
                    if (_stable)
                    {
                        foreach (var project in options.SrcProjects ?? [])
                        {
                            CommandExecutor.ExecuteCommandAndOutput(
                                    $"dotnet pack {project} -o {options.ArtifactsPath}"
                                ).EnsureSuccessExitCode();
                        }
                    }
                    else
                    {
                        var suffix = $"preview-{DateTime.UtcNow:yyyyMMdd-HHmmss}";
                        foreach (var project in options.SrcProjects ?? [])
                        {
                            CommandExecutor.ExecuteCommandAndOutput(
                                $"dotnet pack {project} -o {options.ArtifactsPath} --version-suffix {suffix}"
                                ).EnsureSuccessExitCode();
                        }
                    }

                    if (_noPush)
                    {
                        ConsoleHelper.WriteLineWithColor(@"Skip push there's noPush specified", ConsoleColor.Yellow);
                        return;
                    }

                    if (_branch == "local")
                    {
                        if (!options.AllowLocalPush)
                        {
                            Console.WriteLine(@"Skip push since local branch is not allowed to push packages");
                            return;
                        }
                    }
                    else
                    {
                        // check preview branch
                        if (!_stable && _branch is not "preview")
                        {
                            Console.WriteLine($@"Skip push since branch [{_branch}] not supported to push packages");
                            return;
                        }
                    }

                    if (string.IsNullOrEmpty(_apiKey))
                    {
                        // try to get apiKey from environment variable
                        _apiKey = options.FallbackNuGetApiKeyFunc();

                        if (string.IsNullOrEmpty(_apiKey))
                        {
                            ConsoleHelper.WriteLineWithColor(@"Skip push since there's no apiKey found", ConsoleColor.Yellow);
                            return;
                        }
                    }


                    // push nuget packages
                    var nugetSource =  string.IsNullOrEmpty(_source) ? options.FallbackNuGetSourceFunc() : _source;
                    nugetSource = string.IsNullOrEmpty(nugetSource) ? string.Empty : $"--source {nugetSource}";
                    var pushArguments = $" -k {_apiKey} --skip-duplicate {nugetSource}";
                    foreach (var file in Directory.GetFiles(options.ArtifactsPath, "*.nupkg"))
                    {
                        var commandText = $"dotnet nuget push {file} {pushArguments}";
                        CommandExecutor.ExecuteCommandAndOutput(commandText).EnsureSuccessExitCode();
                    }
                }))
            .WithTask("Default", b => b.WithDependency("pack"))
            ;
#if NET
        foreach (var (task, configure) in options.TaskOverride)
        {
#else
        foreach (var pair in options.TaskOverride)
        {
            var task = pair.Key;
            var configure = pair.Value;
#endif
            builder.WithTask(task, configure);
        }

        _buildProcess = ((BuildProcessBuilder)builder).Build();
    }

    public async Task ExecuteAsync(string[] args, CancellationToken cancellation = default)
    {
        _apiKey = CommandLineParser.Val(args, "apiKey");
        _source = CommandLineParser.Val(args, "source");
        _stable = CommandLineParser.BooleanVal(args, "stable");
        _noPush = CommandLineParser.BooleanVal(args, "noPush");
        if (string.IsNullOrEmpty(_branch))
        {
            _branch = CommandLineParser.Val(args, "branch", "local");
        }
        Debug.Assert(_branch is not null);
        if (!_stable)
        {
            _stable = _branch is "main" or "master" || 
                      _branch?.StartsWith("release/", StringComparison.OrdinalIgnoreCase) == true;
        }
        else
        {
            // only specify `stable` locally
            _stable = _branch == "local";
        }
        var target = CommandLineParser.Val(args, "target", "Default");
        await _buildProcess.ExecuteAsync(target, cancellation);
    }
    
    public static DotNetPackageBuildProcess Create(Action<DotNetBuildProcessOptions> configure)
    {
        var options = new DotNetBuildProcessOptions();
        configure(options);
        return new DotNetPackageBuildProcess(options);
    }
}
