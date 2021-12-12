namespace WeihanLi.Common;

public interface IScope : IDisposable
{
}

public sealed class NullScope : IScope
{
    public void Dispose()
    {
    }

    public static NullScope Instance { get; } = new();
}
