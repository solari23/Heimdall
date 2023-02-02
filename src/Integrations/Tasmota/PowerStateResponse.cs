// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Text.Json.Serialization;

namespace Heimdall.Integrations.Tasmota;

/// <summary>
/// Docs: https://tasmota.github.io/docs/Commands/
/// </summary>
public class PowerStateResponse
{
    public const string PowerStateOn = "ON";
    public const string PowerStateOff = "Off";

    [JsonPropertyName("POWER")]
    public string Power { get; set; }
}
