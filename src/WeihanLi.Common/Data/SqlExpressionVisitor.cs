using System.Linq.Expressions;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Data;

public class SqlExpressionVisitor : ExpressionVisitor
{
    private readonly Stack<string> _leaves = new();
    private readonly IDictionary<string, string>? _columnMappings;

    public SqlExpressionVisitor(IDictionary<string, string>? columnMappings)
    {
        _columnMappings = columnMappings;
    }

    public string GetCondition() => _leaves.StringJoin(" ");

    protected override Expression VisitBinary(BinaryExpression node)
    {
        string GetOperator()
        {
            return node.NodeType switch
            {
                ExpressionType.LeftShift => "<<",
                ExpressionType.RightShift => ">>",
                ExpressionType.AndAlso => "AND",
                ExpressionType.OrElse => "OR",
                ExpressionType.And => "&",
                ExpressionType.Or => "|",
                ExpressionType.GreaterThan => ">",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.LessThan => "<",
                ExpressionType.LessThanOrEqual => "<=",
                ExpressionType.Equal => "=",
                ExpressionType.NotEqual => "!=",
                ExpressionType.Add => "+",
                ExpressionType.Subtract => "-",
                ExpressionType.Multiply => "*",
                ExpressionType.Divide => "/",
                ExpressionType.Modulo => "%",
                _ => node.NodeType.ToString()
            };
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
            return $"{node.Value.ToString()?.Replace("'", "''")}";
        }
        _leaves.Push(GetResult());

        return base.VisitConstant(node);
    }

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
}
