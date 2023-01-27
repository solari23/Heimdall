// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Heimdall.Models.Webhooks;

public class ActionPolymorphicJsonConverter : JsonConverter<IAction>
{
    public override bool CanConvert(Type t) => t.IsAssignableFrom(typeof(IAction));

    public override IAction Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (JsonDocument.TryParseValue(ref reader, out var doc))
        {
            if (doc.RootElement.TryGetPropertyCaseInsensitive(
                nameof(IAction.ActionKind),
                out var kindNode))
            {
                var rawKind = kindNode.GetString();
                if (!Enum.TryParse(rawKind, ignoreCase: true, out ActionKind kind))
                {
                    throw new JsonException($"Value '{rawKind}' is not a valid {nameof(ActionKind)}");
                }

                var rootElement = doc.RootElement.GetRawText();

                return kind switch
                {
                    ActionKind.ToggleSwitch => JsonSerializer.Deserialize<ToggleSwitchAction>(rootElement, options),
                    ActionKind.SetSwitchState => JsonSerializer.Deserialize<SetSwitchStateAction>(rootElement, options),
                    _ => throw new JsonException($"Action kind '{kind}' is not currently supported"),
                };
            }

            throw new JsonException($"Action is missing required field '{nameof(IAction.ActionKind)}'");
        }

        throw new JsonException("Failed to parse Action");
    }

    public override void Write(Utf8JsonWriter writer, IAction value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}
