using System;
using System.Linq.Expressions;

namespace WeihanLi.Common.Helpers
{
    public static class NewFuncHelper<T>
    {
        public static readonly Func<T> Instance = Expression.Lambda<Func<T>>
        (
            Expression.New(typeof(T))
        ).Compile();
    }
}
