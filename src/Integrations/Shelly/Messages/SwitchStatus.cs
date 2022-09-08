// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Text.Json.Serialization;

namespace Heimdall.Integrations.Shelly.Messages;

/// <summary>
/// See: https://shelly-api-docs.shelly.cloud/gen2/ComponentsAndServices/Switch#status
/// </summary>
public class SwitchStatus
{
    [JsonPropertyName("id")]
    public int SwitchId { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; }

    [JsonPropertyName("output")]
    public bool IsOn { get; set; }

    [JsonPropertyName("apower")]
    public double ActivePowerWatts { get; set; }

    [JsonPropertyName("voltage")]
    public double Voltage { get; set; }

    [JsonPropertyName("current")]
    public double Current { get; set; }
}

internal class ActiveEnergy
{
    [JsonPropertyName("total")]
    public double TotalConsumedWattHours { get; set; }

    [JsonPropertyName("by_minute")]
    public double[] ConsumedByMinute { get; set; }

    [JsonPropertyName("minute_ts")]
    [JsonConverter(typeof(UnixTimeJsonConverter))]
    public DateTimeOffset LastMinuteTime { get; set; }
}
