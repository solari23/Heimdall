// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models;
using Heimdall.Models.Dto;

namespace Heimdall.Integrations.Tasmota;

public class TasmotaDeviceController : ISwitchController
{
    internal TasmotaDeviceController(Device deviceInfo, TasmotaClient client)
    {
        this.DeviceInfo = deviceInfo;
        this.Client = client;
    }

    private Device DeviceInfo { get; }

    private TasmotaClient Client { get; }

    public async Task<SwitchState> GetCurrentStateAsync(CancellationToken ct = default)
    {
        var currentStatus = await this.Client.GetSwitchStatusAsync(this.GetDeviceUri(), ct);
        return currentStatus.Power.Equals(PowerStateResponse.PowerStateOn, StringComparison.OrdinalIgnoreCase)
            ? SwitchState.On
            : SwitchState.Off;
    }

    public async Task TurnOnAsync(CancellationToken ct = default)
    {
        await this.Client.SetSwitchAsync(this.GetDeviceUri(), setOn: true, cancellationToken: ct);
    }

    public async Task TurnOffAsync(CancellationToken ct = default)
    {
        await this.Client.SetSwitchAsync(this.GetDeviceUri(), setOn: false, cancellationToken: ct);
    }

    public async Task ToggleAsync(CancellationToken ct = default)
    {
        await this.Client.ToggleSwitchAsync(this.GetDeviceUri(), cancellationToken: ct);
    }

    private Uri GetDeviceUri()
    {
        var builder = new UriBuilder
        {
            Scheme = "http",
            Host = this.DeviceInfo.HostOrIPAddress,
        };
        return builder.Uri;
    }
}
