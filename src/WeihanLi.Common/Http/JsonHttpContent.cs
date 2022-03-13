// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the MIT license.

using Newtonsoft.Json;
using System.Text;

namespace WeihanLi.Common.Http;

public sealed class JsonHttpContent : StringContent
{
    private const string JsonMediaType = "application/json";

    public JsonHttpContent(object obj, JsonSerializerSettings? jsonSerializerSettings = null)
        : this(JsonConvert.SerializeObject(obj, jsonSerializerSettings))
    {
    }

    public JsonHttpContent(string content) : this(content, Encoding.UTF8)
    {
    }

    public JsonHttpContent(string content, Encoding encoding) : base(content, encoding, JsonMediaType)
    {
    }

    public static HttpContent From(object? obj, JsonSerializerSettings? serializerSettings = null)
    {
        if (obj is null)
        {
            return new StringContent(string.Empty, Encoding.UTF8, JsonMediaType);
        }
        return new JsonHttpContent(obj, serializerSettings);
    }
}
