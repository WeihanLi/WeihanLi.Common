// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the MIT license.

using Newtonsoft.Json;
using System.Text;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Http;

public sealed class JsonHttpContent : StringContent
{
    public JsonHttpContent(object obj, JsonSerializerSettings? jsonSerializerSettings = null)
        : this(JsonConvert.SerializeObject(obj, jsonSerializerSettings))
    {
    }

    public JsonHttpContent(string content) : this(content, Encoding.UTF8)
    {
    }

    public JsonHttpContent(string content, Encoding encoding) : base(content, encoding, HttpHelper.ApplicationJsonMediaType)
    {
    }

    public static HttpContent From(object? obj, JsonSerializerSettings? serializerSettings = null)
    {
        if (obj is null)
        {
            return new StringContent(string.Empty, Encoding.UTF8, HttpHelper.ApplicationJsonMediaType);
        }
        return new JsonHttpContent(obj, serializerSettings);
    }
}
