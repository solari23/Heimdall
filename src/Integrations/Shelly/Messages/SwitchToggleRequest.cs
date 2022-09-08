// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Text.Json.Serialization;

namespace Heimdall.Integrations.Shelly.Messages;

public class SwitchToggleRequest : ShellyRequestMessage<SwitchPreviousState>
{
    public override string MethodName => "Switch.Toggle";

    [JsonPropertyName("id")]
    public int SwitchId { get; set; } = 0;
}
