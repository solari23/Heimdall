// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models;

namespace Heimdall.Server.Storage;

public interface IStorageAccess
{
    Task<QueryResult<Device>> GetDeviceByIdAsync(string deviceId, CancellationToken ct = default);

    Task<QueryResult<List<Device>>> GetDevicesAsync(params DeviceType[] typeFilter);

    Task<QueryResult<List<Device>>> GetDevicesAsync(CancellationToken ct, params DeviceType[] typeFilter);

    Task AddDeviceAsync(Device device);

    Task DeleteDeviceAsync(string deviceId);
}
