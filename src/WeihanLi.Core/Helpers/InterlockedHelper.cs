namespace WeihanLi.Common.Helpers;

public static class InterlockedHelper
{
    public static int Read(ref int value)
    {
        return Interlocked.CompareExchange(ref value, 0, 0);
    }

    public static T? Read<T>(ref T? value) where T : class
    {
        return Interlocked.CompareExchange(ref value, null, null);
    }
}
