using System.Net;
using Newtonsoft.Json;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers
{
    public static class ConvertHelper
    {
        #region Json

        /// <summary>
        /// 将object对象转换为Json数据
        /// </summary>
        /// <param name="obj">object对象</param>
        /// <returns>转换后的json字符串</returns>
        public static string ObjectToJson(object obj)
            => ObjectToJson(obj, false, null);

        /// <summary>
        /// 将object对象转换为Json数据
        /// </summary>
        /// <param name="obj">object对象</param>
        /// <param name="serializerSettings">序列化设置</param>
        /// <returns>转换后的json字符串</returns>
        public static string ObjectToJson(object obj, JsonSerializerSettings serializerSettings)
            => ObjectToJson(obj, false, serializerSettings);

        /// <summary>
        /// 将object对象转换为Json数据
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="isConvertToSingleQuotes">是否将双引号转成单引号</param>
        public static string ObjectToJson(object obj, bool isConvertToSingleQuotes)
            => ObjectToJson(obj, isConvertToSingleQuotes, null);

        /// <summary>
        /// 将object对象转换为Json数据
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="isConvertToSingleQuotes">是否将双引号转成单引号</param>
        /// <param name="settings">serializerSettings</param>
        public static string ObjectToJson(object obj, bool isConvertToSingleQuotes, JsonSerializerSettings settings)
        {
            if (obj == null)
                return string.Empty;
            var result = obj.ToJson(isConvertToSingleQuotes, settings);
            if (isConvertToSingleQuotes)
                result = result?.Replace("\"", "'");
            return result;
        }

        /// <summary>
        /// 将Json对象转换为T对象
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="jsonString">json对象字符串</param>
        /// <returns>由字符串转换得到的T对象</returns>
        public static T JsonToObject<T>(string jsonString)
            => JsonToObject<T>(jsonString, null);

        /// <summary>
        /// 将Json对象转换为T对象
        /// </summary>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="jsonString">json对象字符串</param>
        /// <param name="settings">JsonSerializerSettings</param>
        /// <returns>由字符串转换得到的T对象</returns>
        public static T JsonToObject<T>(string jsonString, JsonSerializerSettings settings)
            => string.IsNullOrWhiteSpace(jsonString) ? default(T) : (typeof(T).IsBasicType() ? jsonString.To<T>() : jsonString.JsonToType<T>(settings));

        #endregion Json

        /// <summary>
        /// ip或域名转换为 EndPoint
        /// </summary>
        /// <param name="ipOrHost">ipOrHost</param>
        /// <param name="port">port</param>
        /// <returns>EndPoint</returns>
        public static EndPoint ToEndPoint(string ipOrHost, int port)
        {
            if (IPAddress.TryParse(ipOrHost, out var address))
            {
                return new IPEndPoint(address, port);
            }
            return new DnsEndPoint(ipOrHost, port);
        }
    }
}
