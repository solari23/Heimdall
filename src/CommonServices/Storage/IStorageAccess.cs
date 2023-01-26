// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models;
using Heimdall.Models.Webhooks;

namespace Heimdall.CommonServices.Storage;

public interface IStorageAccess
{
    Task<QueryResult<Device>> GetDeviceByIdAsync(string deviceId, CancellationToken ct = default);

    Task<QueryResult<List<Device>>> GetDevicesAsync(params DeviceType[] typeFilter);

    Task<QueryResult<List<Device>>> GetDevicesAsync(CancellationToken ct, params DeviceType[] typeFilter);

    Task AddDeviceAsync(Device device);

    Task<bool> DeleteDeviceAsync(string deviceId);

    Task<QueryResult<Webhook>> GetWebhookByIdAsync(string webhookId, CancellationToken ct = default);

    Task<QueryResult<List<Webhook>>> GetWebhooksAsync(CancellationToken ct = default);

    Task AddWebhookAsync(Webhook webhook);

    Task<bool> UpdateWebhookAsync(Webhook webhook);

    Task<bool> DeleteWebhookAsync(string webhookId);
}
