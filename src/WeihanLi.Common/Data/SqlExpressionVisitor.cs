using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Data
{
    public class SqlExpressionVisitor : ExpressionVisitor
    {
        private readonly Stack<string> _leaves = new Stack<string>();
        private readonly IDictionary<string, string> _columnMappings;

        public SqlExpressionVisitor(IDictionary<string, string> columnMappings)
        {
            _columnMappings = columnMappings;
        }

        public string GetCondition() => _leaves.StringJoin(" ");

        public override Expression Visit(Expression node) => base.Visit(node);

        protected override Expression VisitBinary(BinaryExpression node)
        {
            string GetOperator()
            {
                switch (node.NodeType)
                {
                    case ExpressionType.LeftShift: return "<<";
                    case ExpressionType.RightShift: return ">>";

                    case ExpressionType.AndAlso: return "AND";
                    case ExpressionType.OrElse: return "OR";

                    case ExpressionType.And: return "&";
                    case ExpressionType.Or: return "|";

                    case ExpressionType.GreaterThan: return ">";
                    case ExpressionType.GreaterThanOrEqual: return ">=";
                    case ExpressionType.LessThan: return "<";
                    case ExpressionType.LessThanOrEqual: return "<=";
                    case ExpressionType.Equal: return "=";
                    case ExpressionType.NotEqual: return "!=";

                    case ExpressionType.Add: return "+";
                    case ExpressionType.Subtract: return "-";
                    case ExpressionType.Multiply: return "*";
                    case ExpressionType.Divide: return "/";
                    case ExpressionType.Modulo: return "%";
                }
                return node.NodeType.ToString();
            }

            // right
            Visit(node.Right);

            var rightMember = _leaves.Peek();
            var oper = GetOperator();
            if (rightMember == "NULL")
            {
                if (node.NodeType == ExpressionType.Equal)
                {
                    _leaves.Push("IS");
                }
                else if (node.NodeType == ExpressionType.NotEqual)
                {
                    _leaves.Push("IS NOT");
                }
                else
                {
                    _leaves.Push(oper);
                }
            }
            else
            {
                _leaves.Push(oper);
            }

            // left
            Visit(node.Left);

            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            string GetResult()
            {
                if (node.Value == null)
                    return "NULL";

                if (node.Value is string strVal)
                {
                    return $"N'{strVal.Replace("'", "''")}'";
                }
                if (node.Value is bool bValue)
                {
                    return bValue ? "1" : "0";
                }
                return $"{node.Value.ToString().Replace("'", "''")}";
            };
            _leaves.Push(GetResult());

            return base.VisitConstant(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node) => base.VisitLambda(node);

        protected override Expression VisitMember(MemberExpression node)
        {
            string GetResult()
            {
                if (node.Member.DeclaringType == typeof(DateTime))
                {
                    return SqlExpressionParser.ParseDateTimeMemberAccess(node, _columnMappings);
                }
                if (node.Member.DeclaringType == typeof(string))
                {
                    return SqlExpressionParser.ParseStringMemberAccess(node, _columnMappings);
                }
                var memberName = node.Member.Name;
                if (_columnMappings != null && _columnMappings.Count > 0)
                {
                    var mapping = _columnMappings.FirstOrDefault(_ => _.Value == memberName);
                    if (mapping.Key.IsNotNullOrWhiteSpace())
                    {
                        return mapping.Key;
                    }
                }
                return memberName;
            }
            _leaves.Push(GetResult());
            return base.VisitMember(node);
        }

        protected override Expression VisitConditional(ConditionalExpression node) => base.VisitConditional(node);

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            string GetResult()
            {
                // TODO:完善 Method Call 解析
                if (node.Object?.Type == typeof(string))
                {
                    return SqlExpressionParser.ParseStringMethodCall(node, _columnMappings);
                }
                if (node.Object?.Type == typeof(DateTime))
                {
                    return SqlExpressionParser.ParseDateTimeMethodCall(node, _columnMappings);
                }
                //
                throw new NotImplementedException();
            }
            _leaves.Push(GetResult());
            return base.VisitMethodCall(node);
        }

        protected override Expression VisitUnary(UnaryExpression node) => base.VisitUnary(node);
    }
}
