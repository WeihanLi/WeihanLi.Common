using System.Collections.Generic;
using System.Reflection;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Aspect
{
    public class FluentAspects
    {
        public static IEntityInterceptionConfiguration<TEntity>
            For<TEntity>()
        {
            //
            return default;
        }
    }

    internal class FluentConfigInterceptorResolver : IInterceptorResolver
    {
        public IReadOnlyCollection<IInterceptor> ResolveInterceptors(IInvocation invocation)
        {
            return ArrayHelper.Empty<IInterceptor>();
        }
    }
}
