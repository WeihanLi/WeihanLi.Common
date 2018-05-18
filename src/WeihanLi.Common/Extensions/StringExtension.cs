using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions
{
    public static class StringExtension
    {
        #region Html Encode Decode

        /// <summary>
        ///     Minimally converts a string to an HTML-encoded string.
        /// </summary>
        /// <param name="s">The string to encode.</param>
        /// <returns>An encoded string.</returns>
        public static string HtmlAttributeEncode([NotNull]this string s)
        {
            return HttpUtility.HtmlAttributeEncode(s);
        }

        /// <summary>
        ///     Converts a string to an HTML-encoded string.
        /// </summary>
        /// <param name="s">The string to encode.</param>
        /// <returns>An encoded string.</returns>
        public static string HtmlEncode([NotNull]this string s)
        {
            return HttpUtility.HtmlEncode(s);
        }

        /// <summary>
        ///     Converts a string that has been HTML-encoded for HTTP transmission into a decoded string.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>A decoded string.</returns>
        public static string HtmlDecode([NotNull]this string s)
        {
            return HttpUtility.HtmlDecode(s);
        }

        /// <summary>
        ///     Encodes a string.
        /// </summary>
        /// <param name="value">A string to encode.</param>
        /// <returns>An encoded string.</returns>
        public static string JavaScriptStringEncode([NotNull]this string value)
        {
            return HttpUtility.JavaScriptStringEncode(value);
        }

        /// <summary>
        ///     Encodes a string.
        /// </summary>
        /// <param name="value">A string to encode.</param>
        /// <param name="addDoubleQuotes">
        ///     A value that indicates whether double quotation marks will be included around the
        ///     encoded string.
        /// </param>
        /// <returns>An encoded string.</returns>
        public static string JavaScriptStringEncode([NotNull]this string value, bool addDoubleQuotes)
        {
            return HttpUtility.JavaScriptStringEncode(value, addDoubleQuotes);
        }

        /// <summary>
        ///     Parses a query string into a  using  encoding.
        /// </summary>
        /// <param name="query">The query string to parse.</param>
        /// <returns>A  of query parameters and values.</returns>
        public static NameValueCollection ParseQueryString([NotNull]this string query)
        {
            return HttpUtility.ParseQueryString(query);
        }

        /// <summary>
        ///     Parses a query string into a  using the specified .
        /// </summary>
        /// <param name="query">The query string to parse.</param>
        /// <param name="encoding">The  to use.</param>
        /// <returns>A  of query parameters and values.</returns>
        public static NameValueCollection ParseQueryString([NotNull]this string query, Encoding encoding)
        {
            return HttpUtility.ParseQueryString(query, encoding);
        }

        /// <summary>
        ///     Converts a string that has been encoded for transmission in a URL into a decoded string.
        /// </summary>
        /// <param name="str">The string to decode.</param>
        /// <returns>A decoded string.</returns>
        public static string UrlDecode([NotNull]this string str)
        {
            return HttpUtility.UrlDecode(str);
        }

        /// <summary>
        ///     Converts a URL-encoded string into a decoded string, using the specified encoding object.
        /// </summary>
        /// <param name="str">The string to decode.</param>
        /// <param name="e">The  that specifies the decoding scheme.</param>
        /// <returns>A decoded string.</returns>
        public static string UrlDecode([NotNull]this string str, Encoding e)
        {
            return HttpUtility.UrlDecode(str, e);
        }

        /// <summary>
        ///     Encodes a URL string.
        /// </summary>
        /// <param name="str">The text to encode.</param>
        /// <returns>An encoded string.</returns>
        public static string UrlEncode([NotNull]this string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        /// <summary>
        ///     Encodes a URL string using the specified encoding object.
        /// </summary>
        /// <param name="str">The text to encode.</param>
        /// <param name="e">The  object that specifies the encoding scheme.</param>
        /// <returns>An encoded string.</returns>
        public static string UrlEncode([NotNull]this string str, Encoding e)
        {
            return HttpUtility.UrlEncode(str, e);
        }

        #endregion Html Encode Decode

        /// <summary>
        /// 根据TypeName获取相应的Type
        /// 支持别名获取，如 int => System.Int32
        /// </summary>
        /// <param name="typeName">typename</param>
        /// <returns></returns>
        public static Type GetTypeByTypeName([NotNull]this string typeName)
        {
            switch (typeName.ToLower())
            {
                case "bool":
                    return Type.GetType("System.Boolean", true, true);

                case "byte":
                    return Type.GetType("System.Byte", true, true);

                case "sbyte":
                    return Type.GetType("System.SByte", true, true);

                case "char":
                    return Type.GetType("System.Char", true, true);

                case "decimal":
                    return Type.GetType("System.Decimal", true, true);

                case "double":
                    return Type.GetType("System.Double", true, true);

                case "float":
                    return Type.GetType("System.Single", true, true);

                case "int":
                    return Type.GetType("System.Int32", true, true);

                case "uint":
                    return Type.GetType("System.UInt32", true, true);

                case "long":
                    return Type.GetType("System.Int64", true, true);

                case "ulong":
                    return Type.GetType("System.UInt64", true, true);

                case "object":
                    return Type.GetType("System.Object", true, true);

                case "short":
                    return Type.GetType("System.Int16", true, true);

                case "ushort":
                    return Type.GetType("System.UInt16", true, true);

                case "string":
                    return Type.GetType("System.String", true, true);

                case "datetime":
                    return Type.GetType("System.DateTime", true, true);

                case "guid":
                    return Type.GetType("System.Guid", true, true);

                default:
                    return Type.GetType(typeName, true, true);
            }
        }

        /// <summary>
        /// Return value if value IsNotNullOrEmpty else return dafaultValue
        /// </summary>
        /// <param name="str">string value</param>
        /// <param name="defaultValue">defaultValue</param>
        /// <returns></returns>
        public static string GetNotEmptyValueOrDefault(this string str, string defaultValue)
        {
            return str.IsNullOrEmpty() ? defaultValue : str;
        }

        /// <summary>
        /// Return value if value IsNotNullOrWhiteSpace else return dafaultValue
        /// </summary>
        /// <param name="str">string value</param>
        /// <param name="defaultValue">defaultValue</param>
        /// <returns></returns>
        public static string GetValueOrDefault(this string str, string defaultValue)
        {
            return str.IsNullOrWhiteSpace() ? defaultValue : str;
        }

        /// <summary>
        /// Return value if value IsNotNullOrWhiteSpace else return dafaultValue
        /// </summary>
        /// <param name="str">string value</param>
        /// <param name="getDefault">get defaultValue func</param>
        /// <returns></returns>
        public static string GetValueOrDefault(this string str, Func<string> getDefault)
        {
            return str.IsNullOrWhiteSpace() ? getDefault() : str;
        }
    }
}
