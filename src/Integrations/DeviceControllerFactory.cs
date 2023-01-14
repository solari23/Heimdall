// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Integrations.Shelly;
using Heimdall.Models;

namespace Heimdall.Integrations;

public interface IDeviceControllerFactory
{
    ISwitchController GetSwitchController(Device device);
}

public class DeviceControllerFactory : IDeviceControllerFactory
{
    public DeviceControllerFactory(ShellyClient shellyClient)
    {
        this.ShellyClient = shellyClient;
    }

    private ShellyClient ShellyClient { get; }

    public ISwitchController GetSwitchController(Device device) 
        => device.Type switch
        {
            DeviceType.ShellyPlug => new ShellyDeviceController(device, this.ShellyClient),
            _ => throw new ArgumentException($"Switch Controller not available for '{device.Type}' devices.", nameof(device)),
        };
}
