// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.ComponentModel.DataAnnotations;

using Heimdall.Models.Events;

namespace Heimdall.Models.Requests;

public class PublishEventRequest
{
#if DEBUG
    public DateTimeOffset? TimeOverrideForTesting { get; set; }
#endif

    [Required]
    public HeimdallEventCategory Category { get; set; }

    [Required]
    public string EventType { get; set; }

    public string MessageTemplate { get; set; }

    public Dictionary<string, string> TemplateParameters { get; set; }
}
