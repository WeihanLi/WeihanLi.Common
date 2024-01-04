// Copyright (c) 2022-2023 Weihan Li. All rights reserved.
// Licensed under the Apache license version 2.0 http://www.apache.org/licenses/LICENSE-2.0

using Newtonsoft.Json;

//
var target = Guard.NotNull(Argument("target", "Default"));
var apiKey = Argument("apiKey", "");
var stable = ArgumentBool("stable", false);
var noPush = ArgumentBool("noPush", false);
var branchName = Environment.GetEnvironmentVariable("BUILD_SOURCEBRANCHNAME") ?? "local";

var solutionPath = "./WeihanLi.Common.sln";
string[] srcProjects = [ 
    "./src/WeihanLi.Common/WeihanLi.Common.csproj",
    "./src/WeihanLi.Common.Logging.Serilog/WeihanLi.Common.Logging.Serilog.csproj",
    "./src/WeihanLi.Extensions.Hosting/WeihanLi.Extensions.Hosting.csproj",
];
string[] testProjects = [ "./test/WeihanLi.Common.Test/WeihanLi.Common.Test.csproj" ];

await BuildProcess.CreateBuilder()
    .WithSetup(() =>
    {
        // cleanup artifacts
        if (Directory.Exists("./artifacts/packages"))
            Directory.Delete("./artifacts/packages", true);

        // args
        Console.WriteLine("Arguments");
        Console.WriteLine($"    {args.StringJoin(" ")}");

        // dump runtime info
        Console.WriteLine("RuntimeInfo:");
        Console.WriteLine(ApplicationHelper.RuntimeInfo.ToJson(new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented
        }));
    })
    .WithTask("hello", b => b.WithExecution(() => Console.WriteLine("Hello dotnet-exec build")))
    .WithTask("build", b =>
    {
        b.WithDescription("dotnet build")
            .WithExecution(() => ExecuteCommandAsync($"dotnet build {solutionPath}"))
            ;
    })
    .WithTask("test", b =>
    {
        b.WithDescription("dotnet test")
            .WithDependency("build")
            .WithExecution(async () =>
            {
                foreach (var project in testProjects)
                {
                    await ExecuteCommandAsync($"dotnet test {project}");
                }
            })
            ;
    })
    .WithTask("pack", b => b.WithDescription("dotnet pack")
        .WithDependency("test")
        .WithExecution(async () =>
        {
            if (stable)
            {
                foreach (var project in srcProjects)
                {
                    await ExecuteCommandAsync($"dotnet pack {project} -o ./artifacts/packages");
                }
            }
            else
            {
                var suffix = $"preview-{DateTime.UtcNow:yyyyMMdd-HHmmss}";
                foreach (var project in srcProjects)
                {
                    await ExecuteCommandAsync($"dotnet pack {project} -o ./artifacts/packages --version-suffix {suffix}");
                }
            }            

            if (noPush)
            {
                Console.WriteLine("Skip push there's noPush specified");
                return;
            }
            
            if (string.IsNullOrEmpty(apiKey))
            {
                // try to get apiKey from environment variable
                apiKey = Environment.GetEnvironmentVariable("NuGet__ApiKey");
                
                if (string.IsNullOrEmpty(apiKey))
                {
                    Console.WriteLine("Skip push since there's no apiKey found");
                    return;
                }
            }

            if (branchName != "master" && branchName != "preview")
            {
                Console.WriteLine($"Skip push since branch name {branchName} not support push packages");
                return;
            }

            // push nuget packages
            foreach (var file in Directory.GetFiles("./artifacts/packages/", "*.nupkg"))
            {
                await ExecuteCommandAsync($"dotnet nuget push {file} -k {apiKey} --skip-duplicate", [new("$NuGet__ApiKey", apiKey)]);
            }
        }))
    .WithTask("Default", b => b.WithDependency("hello").WithDependency("pack"))
    .Build()
    .ExecuteAsync(target);


bool ArgumentBool(string argumentName, bool defaultValue = default)
{
    var value = ArgumentInternal(argumentName);
    if (value is null) return defaultValue;
    if (value == string.Empty || value == "1") return true;
    return  value is "0" ? false : bool.Parse(value);
}

string? Argument(string argumentName, string? defaultValue = default)
{
    return ArgumentInternal(argumentName) ?? defaultValue;
}

string? ArgumentInternal(string argumentName)
{
    for (var i = 0; i < args.Length; i++)
    {
        if (args[i] == $"--{argumentName}" || args[i] == $"-{argumentName}")
        {
            if (((i + 1) == args.Length || args[i + 1].StartsWith('-')))
                return string.Empty;

            return args[i + 1];
        }

        if (args[i].StartsWith($"-{argumentName}="))
            return args[i].Substring($"-{argumentName}=".Length);
        
        if (args[i].StartsWith($"--{argumentName}="))
            return args[i].Substring($"--{argumentName}=".Length);
    }

    return null;
}

async Task ExecuteCommandAsync(string commandText, KeyValuePair<string, string>[]? replacements = null)
{
    var commandTextWithReplacements = commandText;
    if (replacements is { Length: > 0})
    {
        foreach (var item in replacements)
        {
            commandTextWithReplacements = commandTextWithReplacements.Replace(item.Value, item.Key);
        }
    }
    Console.WriteLine($"Executing command: \n    {commandTextWithReplacements}");
    Console.WriteLine();
    var result = await CommandExecutor.ExecuteCommandAndOutputAsync(commandText);
    result.EnsureSuccessExitCode();
    Console.WriteLine();
}

file sealed class BuildProcess
{
    public IReadOnlyCollection<BuildTask> Tasks { get; init; } = [];
    public Func<Task>? Setup { private get; init; }
    public Func<Task>? Cleanup { private get; init; }

    public async Task ExecuteAsync(string target)
    {
        var task = Tasks.FirstOrDefault(x => x.Name == target);
        if (task is null)
            throw new InvalidOperationException("Invalid target to execute");
        
        try
        {
            if (Setup != null)
                await Setup.Invoke();
            
            await ExecuteTask(task);
        }
        finally
        {
            if (Cleanup != null)
                await Cleanup.Invoke();
        }                
    }

    private static async Task ExecuteTask(BuildTask task)
    {
        foreach (var dependencyTask in task.Dependencies)
        {
            await ExecuteTask(dependencyTask);
        }

        Console.WriteLine($"===== Task {task.Name} {task.Description} executing ======");
        await task.ExecuteAsync();
        Console.WriteLine($"===== Task {task.Name} {task.Description} executed ======");
    }

    public static BuildProcessBuilder CreateBuilder()
    {
        return new BuildProcessBuilder();
    }
}

file sealed class BuildProcessBuilder
{
    private readonly List<BuildTask> _tasks = [];
    private Func<Task>? _setup, _cleanup;

    public BuildProcessBuilder WithTask(string name, Action<BuildTaskBuilder> buildTaskConfigure)
    {
        var buildTaskBuilder = new BuildTaskBuilder(name);
        buildTaskBuilder.WithTaskFinder(s => _tasks.Find(t => t.Name == s) ?? throw new InvalidOperationException($"No task found with name {s}"));
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

    internal BuildProcess Build()
    {
        return new BuildProcess()
        {
            Tasks = _tasks,
            Setup = _setup,
            Cleanup = _cleanup
        };
    }
}

file sealed class BuildTask(string name, string? description, Func<Task>? execution = null)
{
    public string Name => name;
    public string Description => description ?? name;

    public IReadOnlyCollection<BuildTask> Dependencies { get; init; } = [];

    public Task ExecuteAsync() => execution?.Invoke() ?? Task.CompletedTask;
}

file sealed class BuildTaskBuilder(string name)
{
    private readonly string _name = name;

    private string? _description;
    private Func<Task>? _execution;
    private readonly List<BuildTask> _dependencies = [];

    public BuildTaskBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }
    
    public BuildTaskBuilder WithExecution(Action execution)
    {
        _execution = execution.WrapTask();
        return this;
    }
    public BuildTaskBuilder WithExecution(Func<Task> execution)
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
        var buildTask = new BuildTask(_name, _description, _execution)
        {
            Dependencies = _dependencies
        };
        return buildTask;
    }
}
