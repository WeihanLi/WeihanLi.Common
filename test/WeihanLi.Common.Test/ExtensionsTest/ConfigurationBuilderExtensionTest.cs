using Microsoft.Extensions.Configuration;
using Xunit;

namespace WeihanLi.Common.Test.ExtensionsTest;

public class ConfigurationBuilderExtensionTest
{
    [Fact]
    public void AddDotEnv_ShouldLoadConfigurationValues()
    {
        using var tempDirectory = new TempDirectory();
        File.WriteAllText(tempDirectory.GetPath(".env"), """
            PlainKey=PlainValue
            App__Name=Demo
            """);

        var configuration = new ConfigurationBuilder()
            .AddDotEnv(options =>
            {
                options.WorkingDirectory = tempDirectory.FullName;
            }, optional: false)
            .Build();

        Assert.Equal("PlainValue", configuration["PlainKey"]);
        Assert.Equal("Demo", configuration["App:Name"]);
    }

    [Fact]
    public void AddDotEnv_WhenOptionalAndFileMissing_ShouldNotThrow()
    {
        using var tempDirectory = new TempDirectory();

        var configuration = new ConfigurationBuilder()
            .AddDotEnv(options =>
            {
                options.WorkingDirectory = tempDirectory.FullName;
            })
            .Build();

        Assert.DoesNotContain(configuration.AsEnumerable(), pair => pair.Value is not null);
    }

    [Fact]
    public void AddDotEnv_WhenRecursiveEnabled_ShouldFindParentFile()
    {
        using var tempDirectory = new TempDirectory();
        var nestedDirectory = tempDirectory.GetPath("child");
        Directory.CreateDirectory(nestedDirectory);
        File.WriteAllText(tempDirectory.GetPath(".env"), "Nested__Key=Value");

        var configuration = new ConfigurationBuilder()
            .AddDotEnv(options =>
            {
                options.WorkingDirectory = nestedDirectory;
                options.Recursive = true;
            }, optional: false)
            .Build();

        Assert.Equal("Value", configuration["Nested:Key"]);
    }

    [Fact]
    public async Task AddDotEnv_WhenWatchingEnabled_ShouldReloadConfiguration()
    {
        using var tempDirectory = new TempDirectory();
        var filePath = tempDirectory.GetPath(".env");
        var cancellationToken = TestContext.Current.CancellationToken;
        await File.WriteAllTextAsync(filePath, "Watcher__Value=Before", cancellationToken);

        using var configuration = new ConfigurationBuilder()
            .AddDotEnv(options =>
            {
                options.WorkingDirectory = tempDirectory.FullName;
            }, optional: false, watching: true)
            .Build();

        Assert.Equal("Before", configuration["Watcher:Value"]);

        var reloadTaskSource = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var registration = configuration.GetReloadToken().RegisterChangeCallback(_ => reloadTaskSource.TrySetResult(), null);

        await Task.Delay(250, cancellationToken);
        await File.WriteAllTextAsync(filePath, "Watcher__Value=After", cancellationToken);

        var completedTask = await Task.WhenAny(reloadTaskSource.Task, Task.Delay(TimeSpan.FromSeconds(10), cancellationToken));
        Assert.Same(reloadTaskSource.Task, completedTask);
        Assert.Equal("After", configuration["Watcher:Value"]);
    }
}
