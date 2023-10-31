using WeihanLi.Common;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions.Dump;

public static class DumpExtension
{
    private const string NullValue = "(null)";

    public static void Dump<T>(this T t) => Dump(t, Console.WriteLine);

    public static void Dump<T>(this T t, Action<string> dumpAction)
    {
        Guard.NotNull(dumpAction, nameof(dumpAction))
            .Invoke(t is null ? NullValue : t.ToJsonOrString());
    }

    public static void Dump<T>(this T t, Action<string> dumpAction, Func<T, string> dumpValueFactory)
    {
        Guard.NotNull(dumpAction, nameof(dumpAction))
            .Invoke(Guard.NotNull(dumpValueFactory, nameof(dumpValueFactory)).Invoke(t));
    }

    public static Task DumpAsync<T>(this T t, Func<string, Task> dumpAction)
    {
        return Guard.NotNull(dumpAction, nameof(dumpAction))
            .Invoke(t is null ? NullValue : t.ToJsonOrString());
    }

    public static Task DumpAsync<T>(this T t, Func<string, Task> dumpAction, Func<T, string> dumpValueFactory)
    {
        return Guard.NotNull(dumpAction, nameof(dumpAction))
            .Invoke(Guard.NotNull(dumpValueFactory, nameof(dumpValueFactory)).Invoke(t));
    }

    public static ValueTask DumpAsync<T>(this T t, Func<string, ValueTask> dumpAction)
    {
        return Guard.NotNull(dumpAction, nameof(dumpAction))
            .Invoke(t is null ? NullValue : t.ToJsonOrString());
    }

    public static ValueTask DumpAsync<T>(this T t, Func<string, ValueTask> dumpAction, Func<T, string> dumpValueFactory)
    {
        return Guard.NotNull(dumpAction, nameof(dumpAction))
            .Invoke(Guard.NotNull(dumpValueFactory, nameof(dumpValueFactory)).Invoke(t));
    }
}
