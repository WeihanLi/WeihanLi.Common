using System.Linq.Expressions;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Common.Data;

/// <summary>
/// DateTime Expression Parser
/// </summary>
internal static partial class SqlExpressionParser
{
    public static string ParseDateTimeMemberAccess(MemberExpression exp, IDictionary<string, string>? columnMappings)
    {
        return exp.Member.Name switch
        {
            "Now" => $"GETDATE()",
            "UtcNow" => $"GETUTCDATE()",
            "Today" => "CONVERT(CHAR(10), GETDATE(),120)",
            _ => string.Empty,
        };
    }

    public static string ParseDateTimeMethodCall(MethodCallExpression exp, IDictionary<string, string>? columnMappings)
    {
        return string.Empty;
    }
}
