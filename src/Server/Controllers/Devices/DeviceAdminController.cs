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

    // Placeholder
    [HttpGet("DoThing")]
    [HeimdallRoleAuthorize(HeimdallRole.HomeViewer)]
    public async Task<IActionResult> DoThingAsync()
    {
        await this.StorageAccess.DoThingAsync();
        return this.Ok();
    }
}
