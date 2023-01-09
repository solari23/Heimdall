// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models;

namespace Heimdall.Server.Storage;

public interface IStorageAccess
{
    // Placeholder
    Task DoThingAsync();

    Task<List<Device>> GetDevicesAsync(params DeviceType[] typeFilter);

    Task<List<Device>> GetDevicesAsync(CancellationToken ct, params DeviceType[] typeFilter);

    Task DeleteDeviceAsync(string deviceId, CancellationToken ct = default);

    Task AddDeviceAsync(Device device, CancellationToken ct = default);
}
