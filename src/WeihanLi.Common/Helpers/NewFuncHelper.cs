using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace WeihanLi.Common.Helpers
{
    public static class NewFuncHelper
    {
        private static readonly ConcurrentDictionary<Type, Func<object>> _newFun =
            new ConcurrentDictionary<Type, Func<object>>();

        public static object NewInstance(Type type)
        {
            var factory = _newFun.GetOrAdd(type, t => Expression.Lambda<Func<object>>
            (
                Expression.New(t)
            ).Compile());
            return factory.Invoke();
        }
    }

    public static class NewFuncHelper<T>
    {
        public static readonly Func<T> Instance = Expression.Lambda<Func<T>>
        (
            Expression.New(typeof(T))
        ).Compile();
    }
}
