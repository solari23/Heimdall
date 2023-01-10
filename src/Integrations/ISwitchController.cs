// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models.Dto;

namespace Heimdall.Integrations;

public interface ISwitchController
{
    Task<SwitchState> GetCurrentStateAsync(CancellationToken ct = default);

    Task TurnOnAsync(CancellationToken ct = default);

    Task TurnOffAsync(CancellationToken ct = default);
}
