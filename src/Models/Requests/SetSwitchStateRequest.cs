// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models.Dto;

namespace Heimdall.Models.Requests;

public class SetSwitchStateRequest
{
    public SwitchState State { get; set; }
}
