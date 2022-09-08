// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models;
using Heimdall.Server.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Heimdall.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class TestController : ControllerBase
{
    [HttpGet("AdminNeeded")]
    [HeimdallRoleAuthorize(HeimdallRole.HomeAdmin)]
    public async Task<IActionResult> AdminNeededAsync()
    {
        await Task.Yield();
        return this.Ok("Hello Admin!");
    }

    [HttpGet("ViewerNeeded")]
    [HeimdallRoleAuthorize(HeimdallRole.HomeViewer)]
    public async Task<IActionResult> ViewerNeededAsync()
    {
        await Task.Yield();
        return this.Ok("Hello Viewer!");
    }
}
