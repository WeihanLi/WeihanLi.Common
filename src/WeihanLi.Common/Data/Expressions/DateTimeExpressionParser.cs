using System.Linq.Expressions;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Common.Data
{
    /// <summary>
    /// DateTime Expression Parser
    /// </summary>
#if DEBUG

    public
#else
        internal
#endif
        static partial class SqlExpressionParser
    {
        public static string ParseDateTimeMemberAccess([NotNull]MemberExpression exp)
        {
            switch (exp.Member.Name)
            {
                case "Now":
                    return $"GETDATE()";

                case "UtcNow":
                    return $"GETUTCDATE()";
            }
            return string.Empty;
        }

        public static string ParseDateTimeMethodCall([NotNull]MethodCallExpression exp)
        {
            return string.Empty;
        }
    }
}
