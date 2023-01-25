// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Net.Http.Json;

using Heimdall.Models;

namespace Heimdall.Web.Services;

public class DeviceRepository
{
    public DeviceRepository(IHttpClientFactory httpFactory)
    {
        this.ApiClient = httpFactory.CreateClient(Program.HeimdallApiHttpClientName);
    }

    private HttpClient ApiClient { get; }

    private DevicesCache devicesCache { get; } = new();

    public async Task<IReadOnlyList<Device>> GetDevicesAsync(
        IReadOnlyCollection<DeviceType> deviceTypeFilter = null)
    {
        var devices = await this.devicesCache.GetAsync(this.ApiClient);

        if (devices is not null && deviceTypeFilter is not null)
        {
            devices = devices.Where(d => deviceTypeFilter.Contains(d.Type)).ToList();
        }

        return devices;
    }

    private class DevicesCache
    {
        private static readonly TimeSpan CachePeriod = TimeSpan.FromSeconds(5);

        private List<Device> devices = null;
        private DateTimeOffset lastUpdateTime = default;
        private SemaphoreSlim refreshLock = new SemaphoreSlim(1);

        public async Task<IReadOnlyList<Device>> GetAsync(HttpClient apiClient)
        {
            if (this.IsCacheStale())
            {
                try
                {
                    await this.refreshLock.WaitAsync();

                    if (this.IsCacheStale())
                    {
                        this.devices = await apiClient.GetFromJsonAsync<List<Device>>(
                            "api/admin/devices",
                            options: JsonHelpers.DefaultJsonOptions);
                        this.lastUpdateTime = DateTimeOffset.UtcNow;
                    }
                }
                finally
                {
                    this.refreshLock.Release();
                }
            }

            return this.devices;
        }

        private bool IsCacheStale()
            => this.devices is null 
                || (DateTimeOffset.UtcNow - this.lastUpdateTime) > CachePeriod;
    }
}
