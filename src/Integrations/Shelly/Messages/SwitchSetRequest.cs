// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Text.Json.Serialization;

namespace Heimdall.Integrations.Shelly.Messages;

public class SwitchSetRequest : ShellyRequestMessage<SwitchPreviousState>
{
    public SwitchSetRequest(bool setOn)
    {
        this.SetOn = setOn;
    }

    public override string MethodName => "Switch.Set";

    [JsonPropertyName("id")]
    public int SwitchId { get; set; } = 0;

    [JsonPropertyName("on")]
    public bool SetOn { get; set; }

    /// <summary>
    /// If set, toggles the state back to the previous state after the given number of seconds.
    /// </summary>
    [JsonPropertyName("toggle_after")]
    public double? ToggleBackAfterSeconds { get; set; }
}
