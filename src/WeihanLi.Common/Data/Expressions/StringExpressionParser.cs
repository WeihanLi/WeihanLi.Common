using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Common.Data
{
    /// <summary>
    /// String Expression Parser
    /// </summary>
#if DEBUG

    public
#else
        internal
#endif
    static partial class SqlExpressionParser
    {
        public static string ParseStringMemberAccess([NotNull]MemberExpression exp)
        {
            return string.Empty;
        }

        public static string ParseStringMethodCall([NotNull]MethodCallExpression exp)
        {
            if (exp.Object == null)
            {
                switch (exp.Method.Name)
                {
                    case "IsNullOrEmpty":
                        return "1=1";

                    case "IsNullOrWhitespace":
                        return "1=1";

                    case "IsNotNullOrEmpty":
                        return "1=0";

                    case "IsNotNullOrWhitespace":
                        return "1=0";
                }
            }
            else
            {
                var left = ParseExpression(exp.Object);
                string arg0 = string.Empty;
                if (exp.Arguments.Count > 0)
                {
                    arg0 = ParseExpression(exp.Arguments[0]);
                }

                switch (exp.Method.Name)
                {
                    case "IsNullOrEmpty":
                        return $"ISNULL({left}, '') = ''";

                    case "IsNullOrWhitespace":
                        return $"TRIM(ISNULL({left}, '')) = ''";

                    case "Contains":
                        if (arg0 == "NULL")
                        {
                            return $"{left} IS NULL";
                        }
                        return $"{left} LIKE N'%{(arg0.StartsWith("N'") ? arg0.Replace("N'", "").Replace("'", "") : arg0.Replace("'", ""))}%'";

                    case "StartsWith":
                        if (arg0 == "NULL")
                        {
                            return $"{left} IS NULL";
                        }
                        return $"({left}) LIKE N'{(arg0.StartsWith("N'") ? arg0.Replace("N'", "").Replace("'", "") : arg0.Replace("'", ""))}%'";

                    case "EndWith":
                        if (arg0 == "NULL")
                        {
                            return $"{left} IS NULL";
                        }
                        return $"({left}) LIKE N'%{(arg0.StartsWith("N'") ? arg0.Replace("N'", "").Replace("'", "") : arg0.Replace("'", ""))}'";

                    case "ToLower": return $"LOWER({left})";
                    case "ToUpper": return $"UPPER({left})";

                    case "Trim": return $"TRIM({left})";
                    case "TrimStart": return $"LTRIM({left})";
                    case "TrimEnd": return $"RTRIM({left})";
                    case "Equals":
                        if (arg0 == "NULL")
                        {
                            return $"{left} IS NULL";
                        }
                        return $"({left} = {arg0})";
                }
            }
            throw new NotImplementedException($"Method {exp.Method.Name} had not supported!");
        }
    }
}
