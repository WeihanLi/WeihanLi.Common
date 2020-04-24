using System;
using System.Linq.Expressions;

namespace WeihanLi.Common.Aspect
{
    public interface IEntityInterceptionConfiguration<TEntity>
    {
        IMethodInterceptionConfiguration Method(Expression<Func<TEntity, Expression<MethodCallExpression>>> methodExpression);
    }
}
