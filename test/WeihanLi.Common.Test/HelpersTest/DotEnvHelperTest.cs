using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test.HelpersTest;

public class DotEnvHelperTest
{
    [Fact]
    public void Read_ShouldParseDotEnvFile()
    {
        using var tempDirectory = new TempDirectory();
        File.WriteAllText(tempDirectory.GetPath(".env"), """
            # comment
            First=Value
            Second = "Second Value"
            export Third='third-value'
            Empty=
            First=Overridden
            """);

        var values = DotEnvHelper.Read(options =>
        {
            options.WorkingDirectory = tempDirectory.FullName;
            options.ExportSupport = true;
        });

        Assert.Equal(4, values.Count);
        Assert.Equal("Overridden", values["First"]);
        Assert.Equal("Second Value", values["Second"]);
        Assert.Equal("third-value", values["Third"]);
        Assert.Equal(string.Empty, values["Empty"]);
    }

    [Fact]
    public void Read_WhenRecursiveEnabled_ShouldFindParentFile()
    {
        using var tempDirectory = new TempDirectory();
        var nestedDirectory = tempDirectory.GetPath("child", "nested");
        Directory.CreateDirectory(nestedDirectory);
        File.WriteAllText(tempDirectory.GetPath(".env"), "RecursiveKey=RecursiveValue");

        var values = DotEnvHelper.Read(options =>
        {
            options.WorkingDirectory = nestedDirectory;
            options.Recursive = true;
        });

        Assert.Equal("RecursiveValue", values["RecursiveKey"]);
    }

    [Fact]
    public async Task ReadAsync_ShouldMatchRead()
    {
        using var tempDirectory = new TempDirectory();
        File.WriteAllText(tempDirectory.GetPath(".env"), """
            SyncOnly=Value
            Trimmed = value
            """);

        var syncValues = DotEnvHelper.Read(options =>
        {
            options.WorkingDirectory = tempDirectory.FullName;
            options.ValueConverter = value => value.ToUpperInvariant();
        });

        var asyncValues = await DotEnvHelper.ReadAsync(options =>
        {
            options.WorkingDirectory = tempDirectory.FullName;
            options.ValueConverter = value => value.ToUpperInvariant();
        });

        Assert.Equal(syncValues, asyncValues);
    }

    [Fact]
    public void Load_ShouldSetEnvironmentVariables()
    {
        using var tempDirectory = new TempDirectory();
        const string key = "WEIHANLI_COMMON_DOTENV_LOAD_TEST";
        File.WriteAllText(tempDirectory.GetPath(".env"), $"{key}=LoadedValue");

        try
        {
            DotEnvHelper.Load(options =>
            {
                options.WorkingDirectory = tempDirectory.FullName;
            });

            Assert.Equal("LoadedValue", Environment.GetEnvironmentVariable(key));
        }
        finally
        {
            Environment.SetEnvironmentVariable(key, null);
        }
    }

    [Fact]
    public void Read_WhenFileMissing_ShouldReturnEmptyDictionary()
    {
        using var tempDirectory = new TempDirectory();

        var values = DotEnvHelper.Read(options =>
        {
            options.WorkingDirectory = tempDirectory.FullName;
        });

        Assert.Empty(values);
    }

    [Fact]
    public void Read_WhenMalformedEntryExists_ShouldThrowFormatException()
    {
        using var tempDirectory = new TempDirectory();
        File.WriteAllText(tempDirectory.GetPath(".env"), """
            Valid=Value
            invalid line
            """);

        Assert.Throws<FormatException>(() => DotEnvHelper.Read(options =>
        {
            options.WorkingDirectory = tempDirectory.FullName;
        }));
    }
}
