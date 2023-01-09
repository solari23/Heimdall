// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models;
using Heimdall.Server.Security;
using Heimdall.Server.Storage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Heimdall.Server.Controllers.Devices;

[ApiController]
[Route("api/devices/admin")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
[HeimdallRoleAuthorize(HeimdallRole.UberAdmin)]
public class DeviceAdminController : Controller
{
    public DeviceAdminController(IStorageAccess storageAccess)
    {
        this.StorageAccess = storageAccess;
    }

    private IStorageAccess StorageAccess { get; }

    [HttpGet]
    public async Task<IActionResult> ListAllAsync(CancellationToken ct)
    {
        var devices = await this.StorageAccess.GetDevicesAsync(ct);
        return this.Ok(devices);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody]Device newDeviceRequest)
    {
        // Ignore device ID coming from client, which should be empty.
        // We'll generate a new unique ID.
        var newDevice = newDeviceRequest with { Id = IdGenerator.CreateNewId() };

        await this.StorageAccess.AddDeviceAsync(newDevice);

        // Technically this should return HTTP 201 'Created' with a URI to the resource.
        // We aren't fully implementing REST resources here, though.
        return this.Ok(newDevice);
    }

    [HttpDelete("{deviceId}")]
    public async Task<IActionResult> DeleteAsync(string deviceId)
    {
        await this.StorageAccess.DeleteDeviceAsync(deviceId);
        return this.Ok();
    }
}
