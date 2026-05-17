using WeihanLi.Common;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions;

public static class ParameterDictionaryExtension
{
    /// <summary>
    /// Get param dictionary
    /// </summary>
    [RequiresUnreferencedCode("Parameter object members may be removed by trimming.")]
    public static IDictionary<string, object?> ParseParamDictionary(this object? paramInfo)
    {
        var paramDic = new Dictionary<string, object?>();
        if (paramInfo is null)
        {
            return paramDic;
        }

        var type = paramInfo.GetType();
        if (type.IsValueTuple()) // Tuple
        {
            var fields = CacheUtil.GetTypeFields(type);
            foreach (var field in fields)
            {
                paramDic[field.Name] = field.GetValue(paramInfo);
            }
        }
        else if (paramInfo is IDictionary<string, object?> paramDictionary)
        {
            return paramDictionary;
        }
        else // get properties
        {
            var properties = CacheUtil.GetTypeProperties(type);
            foreach (var property in properties)
            {
                if (property.CanRead)
                {
                    paramDic[property.Name] = property.GetValueGetter()?.Invoke(paramInfo);
                }
            }
        }

        return paramDic;
    }
}
