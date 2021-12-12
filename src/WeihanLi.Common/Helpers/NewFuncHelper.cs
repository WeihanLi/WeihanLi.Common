using System.Linq.Expressions;

namespace WeihanLi.Common.Helpers;

public static class NewFuncHelper<T>
{
    /// <summary>
    /// CreateNewInstance func
    /// T need to have a parameter less constructor
    /// </summary>
    public static readonly Func<T> Instance = Expression.Lambda<Func<T>>
    (
        Expression.New(typeof(T))
    ).Compile();
}
