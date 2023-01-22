// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Text.Json.Serialization;

namespace Heimdall.Models.Webhooks;

[JsonConverter(typeof(ActionPolymorphicJsonConverter))]
public interface IAction
{
    ActionKind ActionKind { get; }
}
