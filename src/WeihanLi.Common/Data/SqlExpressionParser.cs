using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
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
        public static SqlParseResult ParseWhereExpression(Expression exp)
        {
            var sqlText = new StringBuilder("WHERE ");
            var dic = new Dictionary<string, object>();
            if (exp != null && exp.NodeType == ExpressionType.Lambda && exp is LambdaExpression expression)
            {
                var condition = ParseExpression(exp);
                sqlText.Append(condition.IsNullOrWhiteSpace() ? "1=1" : condition);
            }

            return new SqlParseResult(sqlText.ToString(), dic);
        }

#if DEBUG

        public
#else
        private
#endif
            static string ParseExpression(Expression exp)
        {
            if (exp is LambdaExpression lambdaExpression)
            {
                return ParseExpression(lambdaExpression.Body);
            }
            if (exp is BinaryExpression binaryExpression)
            {
                var left = ParseExpression(binaryExpression.Left);
                var oper = GetExpressionOperatorString(binaryExpression);
                var right = ParseExpression(binaryExpression.Right);
                if (left == "NULL")
                {
                    var tmp = right;
                    right = left;
                    left = tmp;
                }
                if (right == "NULL")
                {
                    oper = oper == "=" ? " IS " : " IS NOT ";
                }
                return $"{left} {oper} {right}";
            }
            if (exp is MemberExpression memberExpression)
            {
                return ParseMemberExpression(memberExpression);
            }
            if (exp is MethodCallExpression methodCallExpression)
            {
                return ParseMethodCallExpression(methodCallExpression);
            }
            if (exp is ConstantExpression constantExpression)
            {
                return ParseConstantExpression(constantExpression);
            }
            if (exp is UnaryExpression unaryExpression)
            {
                return ParseExpression(unaryExpression.Operand);
            }

            return string.Empty;
        }

        private static string ParseMemberExpression(MemberExpression exp)
        {
            if (exp == null)
            {
                return string.Empty;
            }
            if (exp.Member.Name == "Now" && exp.Type == typeof(DateTime))
            {
                return "GETDATE()";
            }

            return exp.Member.Name;
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

        private static string ParseMethodCallExpression(MethodCallExpression expression)
        {
            // TODO:完善 Method Call 解析
            if (expression.Object.Type == typeof(string))
            {
                return ParseStringMethodCall(expression);
            }
            //
            throw new NotImplementedException();
            //if (expression.Object.Type == typeof(DateTime))
            //{
            //    return ParseDateTimeMethodCall(expression);
            //}
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
