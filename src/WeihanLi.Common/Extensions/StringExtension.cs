using JetBrains.Annotations;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using WeihanLi.Common.Helpers;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions
{
    public static class StringExtension
    {
        #region Encode/Decode

        /// <summary>
        ///     Converts a string to an HTML-encoded string.
        /// </summary>
        /// <param name="s">The string to encode.</param>
        /// <returns>An encoded string.</returns>
        public static string HtmlEncode([NotNull] this string s)
        {
            return HttpUtility.HtmlEncode(s);
        }

        /// <summary>
        ///     Converts a string that has been HTML-encoded for HTTP transmission into a decoded string.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>A decoded string.</returns>
        public static string HtmlDecode([NotNull] this string s)
        {
            return HttpUtility.HtmlDecode(s);
        }

        /// <summary>
        ///     Encodes a string.
        /// </summary>
        /// <param name="value">A string to encode.</param>
        /// <returns>An encoded string.</returns>
        public static string JavaScriptStringEncode([NotNull] this string value)
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
        public static string JavaScriptStringEncode([NotNull] this string value, bool addDoubleQuotes)
        {
            return HttpUtility.JavaScriptStringEncode(value, addDoubleQuotes);
        }

        /// <summary>
        ///     Parses a query string into a  using  encoding.
        /// </summary>
        /// <param name="query">The query string to parse.</param>
        /// <returns>A  of query parameters and values.</returns>
        public static NameValueCollection ParseQueryString([NotNull] this string query)
        {
            return HttpUtility.ParseQueryString(query);
        }

        /// <summary>
        ///     Parses a query string into a  using the specified .
        /// </summary>
        /// <param name="query">The query string to parse.</param>
        /// <param name="encoding">The  to use.</param>
        /// <returns>A  of query parameters and values.</returns>
        public static NameValueCollection ParseQueryString([NotNull] this string query, Encoding encoding)
        {
            return HttpUtility.ParseQueryString(query, encoding);
        }

        /// <summary>
        ///     Converts a string that has been encoded for transmission in a URL into a decoded string.
        /// </summary>
        /// <param name="str">The string to decode.</param>
        /// <returns>A decoded string.</returns>
        public static string UrlDecode([NotNull] this string str)
        {
            return HttpUtility.UrlDecode(str);
        }

        /// <summary>
        ///     Converts a URL-encoded string into a decoded string, using the specified encoding object.
        /// </summary>
        /// <param name="str">The string to decode.</param>
        /// <param name="e">The  that specifies the decoding scheme.</param>
        /// <returns>A decoded string.</returns>
        public static string UrlDecode([NotNull] this string str, Encoding e)
        {
            return HttpUtility.UrlDecode(str, e);
        }

        /// <summary>
        ///     Encodes a URL string.
        /// </summary>
        /// <param name="str">The text to encode.</param>
        /// <returns>An encoded string.</returns>
        public static string UrlEncode([NotNull] this string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        /// <summary>
        ///     Encodes a URL string using the specified encoding object.
        /// </summary>
        /// <param name="str">The text to encode.</param>
        /// <param name="e">The  object that specifies the encoding scheme.</param>
        /// <returns>An encoded string.</returns>
        public static string UrlEncode([NotNull] this string str, Encoding e)
        {
            return HttpUtility.UrlEncode(str, e);
        }

        /// <summary>
        /// Base64Encode with utf8 encoding
        /// </summary>
        /// <param name="str">source string</param>
        /// <returns>base64 encoded string</returns>
        public static string Base64Encode([NotNull] this string str) => Base64Encode(str, Encoding.UTF8);

        /// <summary>
        /// Base64Encode
        /// </summary>
        /// <param name="str">source string</param>
        /// <param name="encoding">encoding</param>
        /// <returns>base64 encoded string</returns>
        public static string Base64Encode([NotNull] this string str, Encoding encoding) => Convert.ToBase64String(str.GetBytes(encoding));

        /// <summary>
        /// Base64Decode with ytf8 encoding
        /// </summary>
        /// <param name="str">base64 encoded source string</param>
        /// <returns>base64 decoded string</returns>
        public static string Base64Decode([NotNull] this string str) => Base64Decode(str, Encoding.UTF8);

        /// <summary>
        /// Base64Decode
        /// </summary>
        /// <param name="str">base64 encoded source string</param>
        /// <param name="encoding">encoding</param>
        /// <returns>base64 decoded string</returns>
        public static string Base64Decode([NotNull] this string str, Encoding encoding) => Convert.FromBase64String(str).GetString(encoding);

        /// <summary>
        /// Base64UrlEncode
        /// </summary>
        /// <param name="str">source string</param>
        /// <returns>encoded string</returns>
        public static string Base64UrlEncode([NotNull] this string str) => Base64UrlEncodeHelper.Encode(str);

        /// <summary>
        /// Base64UrlEncode
        /// </summary>
        /// <param name="str">base64url encoded string</param>
        /// <returns>decode string</returns>
        public static string Base64UrlDecode([NotNull] this string str) => Base64UrlEncodeHelper.Decode(str);

        #endregion Encode/Decode

        /// <summary>
        /// 根据TypeName获取相应的Type
        /// 支持别名获取，如 int => System.Int32
        /// </summary>
        /// <param name="typeName">typename</param>
        /// <returns></returns>
        public static Type GetTypeByTypeName([NotNull] this string typeName)
        {
            return typeName.ToLower() switch
            {
                "bool" => Type.GetType("System.Boolean", true, true),
                "byte" => Type.GetType("System.Byte", true, true),
                "sbyte" => Type.GetType("System.SByte", true, true),
                "char" => Type.GetType("System.Char", true, true),
                "decimal" => Type.GetType("System.Decimal", true, true),
                "double" => Type.GetType("System.Double", true, true),
                "float" => Type.GetType("System.Single", true, true),
                "int" => Type.GetType("System.Int32", true, true),
                "uint" => Type.GetType("System.UInt32", true, true),
                "long" => Type.GetType("System.Int64", true, true),
                "ulong" => Type.GetType("System.UInt64", true, true),
                "object" => Type.GetType("System.Object", true, true),
                "short" => Type.GetType("System.Int16", true, true),
                "ushort" => Type.GetType("System.UInt16", true, true),
                "string" => Type.GetType("System.String", true, true),
                "datetime" => Type.GetType("System.DateTime", true, true),
                "guid" => Type.GetType("System.Guid", true, true),
                _ => Type.GetType(typeName, true, true),
            };
        }

        /// <summary>
        /// Return value if value IsNotNullOrEmpty else return defaultValue
        /// </summary>
        /// <param name="str">string value</param>
        /// <param name="defaultValue">defaultValue</param>
        /// <returns></returns>
        public static string GetNotEmptyValueOrDefault(this string? str, string defaultValue)
        {
            return str.IsNullOrEmpty() ? defaultValue : str!;
        }

        /// <summary>
        /// Return value if value IsNotNullOrWhiteSpace else return defaultValue
        /// </summary>
        /// <param name="str">string value</param>
        /// <param name="defaultValue">defaultValue</param>
        /// <returns></returns>
        public static string GetValueOrDefault(this string? str, string defaultValue)
        {
            return str.IsNullOrWhiteSpace() ? defaultValue : str!;
        }

        /// <summary>
        /// Return value if value IsNotNullOrWhiteSpace else return defaultValue
        /// </summary>
        /// <param name="str">string value</param>
        /// <param name="getDefault">get defaultValue func</param>
        /// <returns></returns>
        public static string GetValueOrDefault(this string str, Func<string> getDefault)
        {
            return str.IsNullOrWhiteSpace() ? getDefault() : str;
        }

        /// <summary>
        /// split comma separated string to T array
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="str">str</param>
        /// <param name="splitOptions"></param>
        /// <returns></returns>
        public static T[] SplitArray<T>(this string str, StringSplitOptions splitOptions = StringSplitOptions.None) => SplitArray<T>(str, new[] { ',' }, splitOptions);

        /// <summary>
        /// split specific separator separated string to T array
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="str">str</param>
        /// <param name="separators">separators</param>
        /// <param name="splitOptions">splitOptions</param>
        /// <returns></returns>
        public static T[] SplitArray<T>(this string str, char[] separators, StringSplitOptions splitOptions = StringSplitOptions.None)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return Array.Empty<T>();
            }
            return str.Split(separators, splitOptions)
                .Select(_ => _.To<T>())
                .ToArray();
        }

        /// <summary>
        /// remove start if start with
        /// </summary>
        /// <param name="str">str</param>
        /// <param name="start">start to remove</param>
        /// <returns>start removed str</returns>
        public static string TrimStart(this string str, string start)
        {
            if (str.IsNullOrEmpty() || start.IsNullOrEmpty())
            {
                return str;
            }
            if (str.StartsWith(start))
            {
                str = str.Substring(start.Length);
            }
            return str;
        }
    }
}
