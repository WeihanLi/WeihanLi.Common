// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.
#if NET6_0_OR_GREATER

using System.Text.Encodings.Web;
using System.Text.Json;

namespace WeihanLi.Common.Helpers;

public static class JsonHelper
{
    public static JsonSerializerOptions WebOptions => new(JsonSerializerDefaults.Web);
    public static JsonSerializerOptions UnsafeEncoderOptions => 
        new JsonSerializerOptions(JsonSerializerDefaults.General).WithUnsafeEncoder();
    public static JsonSerializerOptions WriteIntendedUnsafeEncoderOptions => 
        new JsonSerializerOptions(JsonSerializerDefaults.General).WithWriteIntended().WithUnsafeEncoder();

    public static JsonSerializerOptions WithWriteIntended(this JsonSerializerOptions jsonSerializerOptions)
    {
        Guard.NotNull(jsonSerializerOptions);
        jsonSerializerOptions.WriteIndented = true;
        return jsonSerializerOptions;
    }
    
    public static JsonSerializerOptions WithUnsafeEncoder(this JsonSerializerOptions jsonSerializerOptions)
    {
        Guard.NotNull(jsonSerializerOptions);
        jsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        return jsonSerializerOptions;
    }
}

#endif
