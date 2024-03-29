﻿// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.CommonServices.Storage;
using Heimdall.Models;
using Heimdall.Server.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Heimdall.Server.Controllers.Admin;

[Authorize]
[ApiController]
[Route("api/admin/devices")]
[RequiredScope(RequiredScopesConfigurationKey = AadHelpers.RequiredScopesConfigKey)]
[HeimdallRoleAuthorize(HeimdallRole.UberAdmin)]
public class DevicesAdminController : Controller
{
    public DevicesAdminController(IMainStorageAccess mainStorage)
    {
        this.MainStorage = mainStorage;
    }

    private IMainStorageAccess MainStorage { get; }

    [HttpGet]
    public async Task<IActionResult> ListAllAsync(CancellationToken ct)
    {
        var deviceQueryResult = await this.MainStorage.GetDevicesAsync(ct);

        var devices = deviceQueryResult.WasFound ? deviceQueryResult.Data : new List<Device>();

        return this.Ok(devices);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody]Device newDeviceRequest)
    {
        // Ignore device ID coming from client, which should be empty.
        // We'll generate a new unique ID.
        var newDevice = newDeviceRequest with { Id = IdGenerator.CreateNewId() };

        await this.MainStorage.AddDeviceAsync(newDevice);

        // Technically this should return HTTP 201 'Created' with a URI to the resource.
        // We aren't fully implementing REST resources here, though.
        return this.Ok(newDevice);
    }

    [HttpDelete("{deviceId}")]
    public async Task<IActionResult> DeleteAsync(string deviceId)
    {
        bool rowDeleted = await this.MainStorage.DeleteDeviceAsync(deviceId);
        return rowDeleted ? this.Ok() : this.NotFound();
    }
}
