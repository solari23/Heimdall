// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Text.Json.Serialization;

namespace Heimdall.Integrations.Shelly.Messages;

public class SwitchPreviousState
{
    [JsonPropertyName("was_on")]
    public bool WasOn { get; set; }
}
