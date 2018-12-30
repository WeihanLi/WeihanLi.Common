using System;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using WeihanLi.Common;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions
{
    public static class ExpressionExtension
    {
        public static Expression<Func<T, bool>> Or<T>([NotNull]this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>([NotNull]this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }

        /// <summary>
        /// GetMemberName
        /// </summary>
        /// <typeparam name="TEntity">TEntity</typeparam>
        /// <typeparam name="TMember">TMember</typeparam>
        /// <param name="memberExpression">get member expression</param>
        /// <returns></returns>
        public static string
            GetMemberName<TEntity, TMember>([NotNull] this Expression<Func<TEntity, TMember>> memberExpression) =>
            GetMemberInfo(memberExpression).Name;

        /// <summary>
        /// GetMemberInfo
        /// </summary>
        /// <typeparam name="TEntity">TEntity</typeparam>
        /// <typeparam name="TMember">TMember</typeparam>
        /// <param name="expression">get member expression</param>
        /// <returns></returns>
        public static MemberInfo GetMemberInfo<TEntity, TMember>([NotNull]this Expression<Func<TEntity, TMember>> expression)
        {
            if (expression.NodeType != ExpressionType.Lambda)
            {
                throw new ArgumentException(string.Format(Resource.propertyExpression_must_be_lambda_expression, nameof(expression)), nameof(expression));
            }

            var lambda = (LambdaExpression)expression;

            var memberExpression = ExtractMemberExpression(lambda.Body);
            if (memberExpression == null)
            {
                throw new ArgumentException(string.Format(Resource.propertyExpression_must_be_lambda_expression, nameof(memberExpression)), nameof(memberExpression));
            }
            return memberExpression.Member;
        }

        private static MemberExpression ExtractMemberExpression(Expression expression)
        {
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                return (MemberExpression)expression;
            }

            if (expression.NodeType == ExpressionType.Convert)
            {
                var operand = ((UnaryExpression)expression).Operand;
                return ExtractMemberExpression(operand);
            }

            return null;
        }
    }
}
