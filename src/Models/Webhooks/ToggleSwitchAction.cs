// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.ComponentModel.DataAnnotations;

namespace Heimdall.Models.Webhooks;

public record ToggleSwitchAction : IAction
{
    public ActionKind ActionKind => ActionKind.ToggleSwitch;

    [Required]
    public string TargetDeviceId { get; set; }
}
