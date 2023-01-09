// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

namespace Heimdall.Models;

public record Device
{
    public string Id { get; init; }

    public DeviceType Type { get; init; }

    public string Name { get; init; }

    public string HostOrIPAddress { get; init; }
}
