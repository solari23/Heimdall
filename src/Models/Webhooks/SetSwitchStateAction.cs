// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.ComponentModel.DataAnnotations;

using Heimdall.Models.Dto;

namespace Heimdall.Models.Webhooks;

public class SetSwitchStateAction : IAction
{
    public ActionKind ActionKind => ActionKind.SetSwitchState;

    [Required]
    public string TargetDeviceId { get; set; }

    [Required]
    public SwitchState State { get; set; }
}
