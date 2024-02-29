// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers;

public sealed class BuildProcess
{
    public IReadOnlyCollection<BuildTask> Tasks { get; init; } = [];
    public Func<Task>? Setup { private get; init; }
    public Func<Task>? Cleanup { private get; init; }
    public Func<Task>? Cancelled { private get; init; }

    public async Task ExecuteAsync(string target, CancellationToken cancellationToken)
    {
        var task = Tasks.FirstOrDefault(x => x.Name == target);
        if (task is null)
            throw new InvalidOperationException("Invalid target to execute");

        try
        {
            if (Setup != null)
                await Setup.Invoke();

            await ExecuteTask(task, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            if (Cancelled != null)
                await Cancelled.Invoke();
        }
        finally
        {
            if (Cleanup != null)
                await Cleanup.Invoke();
        }
    }

    private static async Task ExecuteTask(BuildTask task, CancellationToken cancellationToken)
    {
        foreach (var dependencyTask in task.Dependencies)
        {
            await ExecuteTask(dependencyTask, cancellationToken);
        }

        Console.WriteLine($@"===== Task {task.Name} {task.Description} executing ======");
        await task.ExecuteAsync(cancellationToken);
        Console.WriteLine($@"===== Task {task.Name} {task.Description} executed ======");
    }
}

public sealed class BuildProcessBuilder
{
    private readonly List<BuildTask> _tasks = [];
    private Func<Task>? _setup, _cleanup, _cancelled;

    public BuildProcessBuilder WithTask(string name, Action<BuildTaskBuilder> buildTaskConfigure)
    {
        var buildTaskBuilder = new BuildTaskBuilder(name);
        buildTaskBuilder.WithTaskFinder(s =>
            _tasks.Find(t => t.Name == s) ?? throw new InvalidOperationException($"No task found with name {s}"));
        buildTaskConfigure.Invoke(buildTaskBuilder);
        var task = buildTaskBuilder.Build();
        _tasks.Add(task);
        return this;
    }

    public BuildProcessBuilder WithSetup(Action setupFunc)
    {
        _setup = setupFunc.WrapTask();
        return this;
    }

    public BuildProcessBuilder WithSetup(Func<Task> setupFunc)
    {
        _setup = setupFunc;
        return this;
    }

    public BuildProcessBuilder WithCleanup(Action cleanupFunc)
    {
        _cleanup = cleanupFunc.WrapTask();
        return this;
    }

    public BuildProcessBuilder WithCleanup(Func<Task> cleanupFunc)
    {
        _cleanup = cleanupFunc;
        return this;
    }

    public BuildProcessBuilder WithCancelled(Action cancelledFunc)
    {
        _cancelled = cancelledFunc.WrapTask();
        return this;
    }

    public BuildProcessBuilder WithCancelled(Func<Task> cancelledFunc)
    {
        _cancelled = cancelledFunc;
        return this;
    }

    public BuildProcess Build()
    {
        return new BuildProcess
        {
            Tasks = _tasks, 
            Setup = _setup, 
            Cleanup = _cleanup, 
            Cancelled = _cancelled
        };
    }
}

public sealed class BuildTask(string name, string? description, Func<CancellationToken, Task>? execution = null)
{
    public string Name => name;
    public string Description => description ?? name;

    public IReadOnlyCollection<BuildTask> Dependencies { get; init; } = [];

    public Task ExecuteAsync(CancellationToken cancellationToken) =>
        execution?.Invoke(cancellationToken) ?? Task.CompletedTask;
}

public sealed class BuildTaskBuilder(string name)
{
    private readonly string _name = name;

    private string? _description;
    private Func<CancellationToken, Task>? _execution;
    private readonly List<BuildTask> _dependencies = [];

    public BuildTaskBuilder WithDescription(string? description)
    {
        _description = description;
        return this;
    }

    public BuildTaskBuilder WithExecution(Action execution)
    {
        _execution = execution.WrapTask().WrapCancellation();
        return this;
    }

    public BuildTaskBuilder WithExecution(Func<Task> execution)
    {
        _execution = execution.WrapCancellation();
        return this;
    }

    public BuildTaskBuilder WithExecution(Func<CancellationToken, Task> execution)
    {
        _execution = execution;
        return this;
    }

    public BuildTaskBuilder WithDependency(string dependencyTaskName)
    {
        if (_taskFinder is null) throw new InvalidOperationException("Dependency task name is not supported");

        _dependencies.Add(_taskFinder.Invoke(dependencyTaskName));
        return this;
    }

    private Func<string, BuildTask>? _taskFinder;

    internal BuildTaskBuilder WithTaskFinder(Func<string, BuildTask> taskFinder)
    {
        _taskFinder = taskFinder;
        return this;
    }

    public BuildTask Build()
    {
        var buildTask = new BuildTask(_name, _description, _execution) { Dependencies = _dependencies };
        return buildTask;
    }
}
