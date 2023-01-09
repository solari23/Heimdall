// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.ComponentModel.DataAnnotations;

namespace Heimdall.Models;

public record Device
{
    public string Id { get; init; }

    [Required]
    public DeviceType Type { get; init; }

    [Required]
    public string Name { get; init; }

    [Required]
    public string HostOrIPAddress { get; init; }
}
