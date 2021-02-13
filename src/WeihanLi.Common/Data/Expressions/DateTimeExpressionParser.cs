using System.Collections.Generic;
using System.Linq.Expressions;

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
        public static string ParseDateTimeMemberAccess(MemberExpression exp, IDictionary<string, string>? columnMappings)
        {
            switch (exp.Member.Name)
            {
                case "Now":
                    return $"GETDATE()";

                case "UtcNow":
                    return $"GETUTCDATE()";

                case "Today":
                    return "CONVERT(CHAR(10), GETDATE(),120)";
            }
            return string.Empty;
        }

        public static string ParseDateTimeMethodCall(MethodCallExpression exp, IDictionary<string, string>? columnMappings)
        {
            return string.Empty;
        }
    }
}
