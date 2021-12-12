using System.Linq.Expressions;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Data;

internal static partial class SqlExpressionParser
{
    internal static SqlParseResult ParseWhereExpression(Expression? exp, IDictionary<string, string>? columnMappings)
    {
        var sqlText = string.Empty;
        var dic = new Dictionary<string, object?>();
        if (exp != null && exp.NodeType == ExpressionType.Lambda && exp is LambdaExpression)
        {
            var condition = ParseExpression(exp, columnMappings);
            if (condition.IsNotNullOrWhiteSpace())
            {
                sqlText = $"WHERE {condition}";
            }
        }
        return new SqlParseResult(sqlText, dic);
    }

    public static string ParseExpression(Expression? exp, IDictionary<string, string>? columnMappings = null)
    {
        if (exp is LambdaExpression lambdaExpression)
        {
            return ParseExpression(lambdaExpression.Body, columnMappings);
        }
        if (exp is BinaryExpression binaryExpression)
        {
            var left = ParseExpression(binaryExpression.Left, columnMappings);
            var @operator = GetExpressionOperatorString(binaryExpression);
            var right = ParseExpression(binaryExpression.Right, columnMappings);
            if (left == "NULL")
            {
                var tmp = right;
                right = left;
                left = tmp;
            }
            if (right == "NULL")
            {
                @operator = @operator == "=" ? " IS " : " IS NOT ";
            }
            return $"{left} {@operator} {right}";
        }
        if (exp is MemberExpression memberExpression)
        {
            return ParseMemberExpression(memberExpression, columnMappings);
        }
        if (exp is MethodCallExpression methodCallExpression)
        {
            return ParseMethodCallExpression(methodCallExpression, columnMappings);
        }
        if (exp is ConstantExpression constantExpression)
        {
            return ParseConstantExpression(constantExpression);
        }
        if (exp is UnaryExpression unaryExpression)
        {
            return ParseExpression(unaryExpression.Operand, columnMappings);
        }

        return string.Empty;
    }

    private static string ParseMemberExpression(MemberExpression? exp, IDictionary<string, string>? columnMappings)
    {
        if (exp == null)
        {
            return string.Empty;
        }
        if (exp.Member.DeclaringType == typeof(DateTime))
        {
            return ParseDateTimeMemberAccess(exp, columnMappings);
        }
        if (exp.Member.DeclaringType == typeof(string))
        {
            return ParseStringMemberAccess(exp, columnMappings);
        }
        var memberName = exp.Member.Name;
        if (columnMappings != null && columnMappings.Count > 0)
        {
            var mapping = columnMappings.FirstOrDefault(_ => _.Value == memberName);
            if (mapping.Key.IsNotNullOrWhiteSpace())
            {
                return mapping.Key;
            }
        }
        return memberName;
    }

    private static string GetExpressionOperatorString(BinaryExpression exp)
    {
        return exp.NodeType switch
        {
            ExpressionType.OrElse => " OR ",
            ExpressionType.Or => "|",
            ExpressionType.AndAlso => " AND ",
            ExpressionType.And => "&",
            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThan => "<",
            ExpressionType.LessThanOrEqual => "<=",
            ExpressionType.NotEqual => "<>",
            ExpressionType.Add => "+",
            ExpressionType.Subtract => "-",
            ExpressionType.Multiply => "*",
            ExpressionType.Divide => "/",
            ExpressionType.Modulo => "%",
            ExpressionType.Equal => "=",
            _ => exp.NodeType.ToString()
        };
    }

    private static string ParseConstantExpression(ConstantExpression exp)
    {
        if (exp.Value == null)
        {
            return "IS NULL";
        }
        if (exp.Value is string strVal)
        {
            return $"N'{strVal.Replace("'", "''")}'";
        }
        if (exp.Value is bool bValue)
        {
            return bValue ? "1" : "0";
        }
        return $"{exp.Value.ToString()?.Replace("'", "''")}";
    }

    private static string ParseMethodCallExpression(MethodCallExpression expression, IDictionary<string, string>? columnMappings)
    {
        if (expression.Object?.Type == typeof(string))
        {
            return ParseStringMethodCall(expression, columnMappings);
        }
        //
        throw new NotImplementedException();
    }
}

internal class SqlParseResult
{
    public string SqlText { get; }

    public IDictionary<string, object?> Parameters { get; }

    public SqlParseResult(string sqlText, IDictionary<string, object?> parameters)
    {
        SqlText = sqlText;
        Parameters = parameters;
    }
}
