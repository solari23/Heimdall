// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.ComponentModel.DataAnnotations;
using Heimdall.Models.Events;

namespace Heimdall.Models.Webhooks;

public class PublishEventAction : IAction
{
    public ActionKind ActionKind => ActionKind.PublishEvent;

    [Required]
    public HeimdallEventCategory Category { get; set; }

    [Required]
    public string EventType { get; set; }

    public string MessageTemplate { get; set; }
}
