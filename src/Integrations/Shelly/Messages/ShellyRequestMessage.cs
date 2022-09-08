// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Text.Json.Serialization;

namespace Heimdall.Integrations.Shelly.Messages;

public abstract class ShellyRequestMessage<TResponse>
{
    public abstract string MethodName { get; }
}
