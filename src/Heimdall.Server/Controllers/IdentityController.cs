// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models;
using Heimdall.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Heimdall.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class IdentityController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var heimdallRoleClaim = this.User.Claims.FirstOrDefault(c => c.Type == HeimdallRole.ClaimType);

        var idInfo = new IdentityInfo
        {
            HeimdallRole = heimdallRoleClaim?.Value ?? HeimdallRole.NoRole,
        };

        return this.Ok(idInfo);
    }
}
