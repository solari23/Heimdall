// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Text.Json.Serialization;

namespace Heimdall.Integrations.Shelly.Messages;

public class SwitchGetStatusRequest : ShellyRequestMessage<SwitchStatus>
{
    public override string MethodName => "Switch.GetStatus";

    [JsonPropertyName("id")]
    public int SwitchId { get; set; } = 0;
}
