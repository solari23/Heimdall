// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models;
using Heimdall.Models.Dto;

namespace Heimdall.Integrations.Shelly;

public class ShellyDeviceController : ISwitchController
{
    internal ShellyDeviceController(Device deviceInfo, ShellyClient client)
    {
        this.DeviceInfo = deviceInfo;
        this.Client = client;
    }

    private Device DeviceInfo { get; }

    private ShellyClient Client { get; }

    public async Task<SwitchState> GetCurrentStateAsync(CancellationToken ct = default)
    {
        var currentStatus = await this.Client.GetSwitchStatusAsync(this.GetDeviceUri(), ct);
        return currentStatus.IsOn ? SwitchState.On : SwitchState.Off;
    }

    public async Task TurnOnAsync(CancellationToken ct = default)
    {
        await this.Client.SetSwitchAsync(this.GetDeviceUri(), setOn: true, cancellationToken: ct);
    }

    public async Task TurnOffAsync(CancellationToken ct = default)
    {
        await this.Client.SetSwitchAsync(this.GetDeviceUri(), setOn: false, cancellationToken: ct);
    }

    private Uri GetDeviceUri()
    {
        var builder = new UriBuilder();
        builder.Scheme = "http";
        builder.Host = this.DeviceInfo.HostOrIPAddress;
        return builder.Uri;
    }
}
