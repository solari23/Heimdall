// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.ComponentModel.DataAnnotations;

namespace Heimdall.Models.Events;

public record HeimdallEvent
{
    /// <summary>
    /// The UTC timestamp of the event.
    /// </summary>
    [Required]
    public DateTimeOffset TimeUtc { get; set; }

    /// <summary>
    /// The Heimdall standard category of the event.
    /// </summary>
    [Required]
    public HeimdallEventCategory Category { get; set; }

    /// <summary>
    /// An event type identifier provided by the publisher.
    /// </summary>
    [Required]
    public string EventType { get; set; }

    /// <summary>
    /// An optional textual message describing the event.
    /// </summary>
    public string Message { get; set; }
}
