// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.ComponentModel.DataAnnotations;

namespace Heimdall.Models.Webhooks;

public record Webhook
{
    public string Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public List<IAction> Actions { get; set; } = new();
}
