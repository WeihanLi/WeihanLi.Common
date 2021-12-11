using System;
using System.Linq.Expressions;

namespace WeihanLi.Common.Helpers;

public static class ExpressionHelper
{
    public static Expression<Func<T, bool>> True<T>()
    {
        return ConstantExpressions<T>.TrueExpression;
    }

    public static Expression<Func<T, bool>> False<T>()
    {
        return ConstantExpressions<T>.FalseExpression;
    }

    private static class ConstantExpressions<T>
    {
        public static readonly Expression<Func<T, bool>> TrueExpression = t => true;

        public static readonly Expression<Func<T, bool>> FalseExpression = t => false;
    }
}
