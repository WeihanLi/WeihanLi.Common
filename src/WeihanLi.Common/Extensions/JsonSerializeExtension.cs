using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using WeihanLi.Common;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions;

public static class JsonSerializeExtension
{
    /// <summary>
    /// DefaultSerializerSettings
    /// </summary>
    private static readonly JsonSerializerSettings DefaultSerializerSettings = GetDefaultSerializerSettings();

    private static JsonSerializerSettings GetDefaultSerializerSettings() => new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        MissingMemberHandling = MissingMemberHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore,
    };

    public static JsonSerializerSettings SerializerSettingsWith(Action<JsonSerializerSettings>? action)
    {
        if (null == action)
            return DefaultSerializerSettings;

        var serializerSettings = GetDefaultSerializerSettings();
        action.Invoke(serializerSettings);
        return serializerSettings;
    }

    /// <summary>
    /// 将object对象转换为Json数据
    /// </summary>
    /// <param name="obj">object对象</param>
    /// <returns>转换后的json字符串</returns>
    public static string ToJson(this object? obj)
        => obj.ToJson(false, null);

    /// <summary>
    /// 将object对象转换为Json数据
    /// </summary>
    /// <param name="obj">object对象</param>
    /// <param name="serializerSettings">序列化设置</param>
    /// <returns>转换后的json字符串</returns>
    public static string ToJson(this object obj, JsonSerializerSettings? serializerSettings)
        => obj.ToJson(false, serializerSettings);

    /// <summary>
    /// 将object对象转换为Json数据
    /// </summary>
    /// <param name="obj">目标对象</param>
    /// <param name="isConvertToSingleQuotes">是否将双引号转成单引号</param>
    public static string ToJson(this object? obj, bool isConvertToSingleQuotes)
        => obj.ToJson(isConvertToSingleQuotes, null);

    /// <summary>
    /// 将object对象转换为Json数据
    /// </summary>
    /// <param name="obj">目标对象</param>
    /// <param name="isConvertToSingleQuotes">是否将双引号转成单引号</param>
    /// <param name="settings">serializerSettings</param>
    public static string ToJson(this object? obj, bool isConvertToSingleQuotes, JsonSerializerSettings? settings)
    {
        if (obj == null)
        {
            return string.Empty;
        }
        var result = JsonConvert.SerializeObject(obj, settings ?? DefaultSerializerSettings) ?? string.Empty;
        if (isConvertToSingleQuotes)
        {
            result = result.Replace("\"", "'");
        }
        return result;
    }

    /// <summary>
    /// 将Json对象转换为T对象
    /// </summary>
    /// <typeparam name="T">对象的类型</typeparam>
    /// <param name="jsonString">json对象字符串</param>
    /// <returns>由字符串转换得到的T对象</returns>
    public static T JsonToObject<T>(this string jsonString)
        => jsonString.JsonToObject<T>(null);

    /// <summary>
    /// 将Json对象转换为T对象
    /// </summary>
    /// <typeparam name="T">对象的类型</typeparam>
    /// <param name="jsonString">json对象字符串</param>
    /// <param name="settings">JsonSerializerSettings</param>
    /// <returns>由字符串转换得到的T对象</returns>
    public static T JsonToObject<T>(this string jsonString, JsonSerializerSettings? settings)
        => Guard.NotNull(JsonConvert.DeserializeObject<T>(jsonString, settings ?? DefaultSerializerSettings));

    /// <summary>
    /// 对象转换为string，如果是基元类型直接ToString(),如果是Entity则Json序列化
    /// </summary>
    /// <param name="obj">要操作的对象</param>
    /// <returns></returns>
    public static string ToJsonOrString(this object? obj)
    {
        if (obj is null)
        {
            return string.Empty;
        }
        if (obj is string str)
        {
            return str;
        }
        if (obj.GetType().IsBasicType())
        {
            return Convert.ToString(obj) ?? string.Empty;
        }
        return obj.ToJson();
    }

    /// <summary>
    /// 字符串数据转换为相应类型的对象，如果是基本数据类型则转换类型，是Entity则Json反序列化
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="jsonString">字符串</param>
    /// <returns></returns>
    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static T StringToType<T>(this string jsonString)
    {
        if (typeof(T) == typeof(string))
        {
            return (T)(object)jsonString;
        }
        if (typeof(T).IsBasicType())
        {
            return jsonString.To<T>();
        }
        return jsonString.JsonToObject<T>();
    }

    /// <summary>
    /// 字符串数据转换为相应类型的对象，如果是基元类型则转换类型，是Entity则Json反序列化
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="jsonString">json string</param>
    /// <param name="defaultValue">defaultValue</param>
    /// <returns></returns>
    public static T StringToType<T>(this string jsonString, T defaultValue)
    {
        if (typeof(T) == typeof(string))
        {
            return (T)(object)jsonString;
        }
        if (typeof(T).IsBasicType())
        {
            return Guard.NotNull(jsonString.ToOrDefault(defaultValue));
        }

        if (string.IsNullOrWhiteSpace(jsonString))
        {
            return defaultValue;
        }
        try
        {
            return jsonString.JsonToObject<T>();
        }
        catch
        {
            return defaultValue;
        }
    }
}
