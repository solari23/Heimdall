// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.ComponentModel.DataAnnotations;

namespace Heimdall.Models;

public record Device
{
    public string Id { get; set; }

    [Required]
    public DeviceType Type { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string HostOrIPAddress { get; set; }
}
