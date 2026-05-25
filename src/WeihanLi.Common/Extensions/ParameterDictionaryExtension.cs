using System.Data;
using WeihanLi.Common;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions;

public static class ParameterDictionaryExtension
{
    /// <summary>
    /// IDictionary to dataTable
    /// </summary>
    /// <param name="dictionary">IDictionary</param>
    /// <returns></returns>
    [RequiresUnreferencedCode("Dictionary value runtime types may not preserve members required by DataColumn.")]
    public static DataTable ToDataTable(this IDictionary<string, object> dictionary)
    {
        Guard.NotNull(dictionary);
        var dataTable = new DataTable();
        if (dictionary.Keys.Count == 0)
        {
            return dataTable;
        }
        dataTable.Columns.AddRange(dictionary.Keys.Select(key => new DataColumn(key, dictionary[key].GetType())).ToArray());
        foreach (var key in dictionary.Keys)
        {
            var row = dataTable.NewRow();
            row[key] = dictionary[key];
            dataTable.Rows.Add(row);
        }
        return dataTable;
    }

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
