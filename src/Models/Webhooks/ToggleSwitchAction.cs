// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

namespace Heimdall.Models.Webhooks;

public class ToggleSwitchAction : IAction
{
    public ActionKind ActionKind => ActionKind.ToggleSwitch;

    public string TargetDeviceId { get; set; }
}
