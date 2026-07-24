using System.Runtime.InteropServices;

namespace WeihanLi.Common.Test;

internal sealed class TempDirectory : IDisposable
{
    public TempDirectory()
    {
        FullName = Path.Combine(Path.GetTempPath(), $"WeihanLi.Common.{Guid.NewGuid():N}");
        Directory.CreateDirectory(FullName);
    }

    public string FullName { get; }

    public string GetPath(params string[] paths)
    {
        return Path.Combine([FullName, .. paths]);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(FullName))
            {
                Directory.Delete(FullName, true);
            }
        }
        catch (IOException) when (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // File watcher tests can keep temporary handles alive briefly on Windows.
        }
        catch (UnauthorizedAccessException) when (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // File watcher tests can keep temporary handles alive briefly on Windows.
        }
    }
}
