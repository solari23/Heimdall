// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Heimdall.Integrations.Shelly.Messages;

public sealed class UnixTimeJsonConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        string rawValue = reader.GetString();
        
        if (!long.TryParse(rawValue, out long value))
        {
            throw new JsonException();
        }

        return DateTimeOffset.FromUnixTimeSeconds(value);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.ToUnixTimeSeconds());
    }
}
