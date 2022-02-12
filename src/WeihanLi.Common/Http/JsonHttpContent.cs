// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the MIT license.

using Newtonsoft.Json;
using System.Text;

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

    public JsonHttpContent(string content, Encoding encoding) : base(content, encoding, "application/json")
    {
    }
}
