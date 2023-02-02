// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Integrations.Shelly;
using Heimdall.Integrations.Tasmota;
using Heimdall.Models;

namespace Heimdall.Integrations;

public interface IDeviceControllerFactory
{
    ISwitchController GetSwitchController(Device device);
}

public class DeviceControllerFactory : IDeviceControllerFactory
{
    public DeviceControllerFactory(
        ShellyClient shellyClient,
        TasmotaClient tasmotaClient)
    {
        this.ShellyClient = shellyClient;
        this.TasmotaClient = tasmotaClient;
    }

    private ShellyClient ShellyClient { get; }

    private TasmotaClient TasmotaClient { get; }

    public ISwitchController GetSwitchController(Device device) 
        => device.Type switch
        {
            DeviceType.ShellyPlug => new ShellyDeviceController(device, this.ShellyClient),
            DeviceType.TasmotaPlug => new TasmotaDeviceController(device, this.TasmotaClient),
            _ => throw new ArgumentException($"Switch Controller not available for '{device.Type}' devices.", nameof(device)),
        };
}
