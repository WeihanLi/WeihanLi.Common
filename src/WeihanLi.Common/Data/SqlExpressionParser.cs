using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Data
{
#if DEBUG

    public
#else

    internal
#endif
        static partial class SqlExpressionParser
    {
        public static SqlParseResult ParseWhereExpression(Expression exp, IDictionary<string, string> columnMappings)
        {
            var sqlText = string.Empty;
            var dic = new Dictionary<string, object>();
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

#if DEBUG

        public
#else

        private
#endif
            static string ParseExpression(Expression exp, IDictionary<string, string> columnMappings = null)
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

        private static string ParseMemberExpression(MemberExpression exp, IDictionary<string, string> columnMappings)
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
            switch (exp.NodeType)
            {
                case ExpressionType.OrElse: return " OR ";
                case ExpressionType.Or: return "|";
                case ExpressionType.AndAlso: return " AND ";
                case ExpressionType.And: return "&";
                case ExpressionType.GreaterThan: return ">";
                case ExpressionType.GreaterThanOrEqual: return ">=";
                case ExpressionType.LessThan: return "<";
                case ExpressionType.LessThanOrEqual: return "<=";
                case ExpressionType.NotEqual: return "<>";
                case ExpressionType.Add: return "+";
                case ExpressionType.Subtract: return "-";
                case ExpressionType.Multiply: return "*";
                case ExpressionType.Divide: return "/";
                case ExpressionType.Modulo: return "%";
                case ExpressionType.Equal: return "=";
            }
            return "";
        }

        private static string ParseConstantExpression(ConstantExpression exp)
        {
            if (exp.Value is string strVal)
            {
                return $"N'{strVal.Replace("'", "''")}'";
            }
            if (exp.Value is bool bValue)
            {
                return bValue ? "1" : "0";
            }
            return $"{exp.Value.ToString().Replace("'", "''")}";
        }

        private static string ParseMethodCallExpression(MethodCallExpression expression, IDictionary<string, string> columnMappings)
        {
            // TODO:完善 Method Call 解析
            if (expression.Object.Type == typeof(string))
            {
                return ParseStringMethodCall(expression, columnMappings);
            }
            //
            throw new NotImplementedException();
            //if (expression.Object.Type == typeof(DateTime))
            //{
            //    return ParseDateTimeMethodCall(expression);
            //}
            // return string.Empty;
        }
    }

#if DEBUG

    public
#else

    internal
#endif
            class SqlParseResult
    {
        public string SqlText { get; }

        public IDictionary<string, object> Parameters { get; }

        public SqlParseResult(string sqlText, IDictionary<string, object> parameters)
        {
            SqlText = sqlText;
            Parameters = parameters;
        }
    }
}
