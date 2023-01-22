// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Heimdall.Models;

public static class JsonHelpers
{
    public static readonly JsonSerializerOptions DefaultJsonOptions = CreateDefaultJsonOptions();

    private static JsonSerializerOptions CreateDefaultJsonOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }

    public static bool TryGetPropertyCaseInsensitive(
        this JsonElement element,
        string propertyName,
        out JsonElement value)
    {
        value = default;

        foreach (var property in element.EnumerateObject().OfType<JsonProperty>())
        {
            if (property.Name.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
            {
                value = property.Value;
                return true;
            }
        }

        return false;
    }
}
