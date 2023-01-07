// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models;
using Heimdall.Models.Dto;
using Heimdall.Server.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Heimdall.Server.Controllers.Devices;

[ApiController]
[Route("api/devices/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class SwitchController : Controller
{
    private static List<SwitchInfo> Switches => new List<SwitchInfo>
    {
        new SwitchInfo
        {
            Id = "FOOBAZ_1",
            Label = "The best switch ever",
            State = SwitchState.Unknown,
        },
        new SwitchInfo
        {
            Id = "FOOBAZ_2",
            Label = "This one is on",
            State = SwitchState.Unknown,
        },
        new SwitchInfo
        {
            Id = "FOOBAZ_3",
            Label = "This one is off",
            State = SwitchState.Unknown,
        },
    };

    [HttpGet("ListAll")]
    [HeimdallRoleAuthorize(HeimdallRole.HomeViewer)]
    public async Task<IActionResult> ListAllAsync()
    {
        await Task.Delay(1500);
        return this.Ok(Switches);
    }

    [HttpGet("{switchId}")]
    [HeimdallRoleAuthorize(HeimdallRole.HomeViewer)]
    public async Task<IActionResult> GetAsync(string switchId)
    {
        var switchInfo = Switches.FirstOrDefault(s => s.Id == switchId);

        if (switchInfo is null)
        {
            return this.NotFound();
        }

        await Task.Delay(1000);

        if (switchId == "FOOBAZ_2")
        {
            await Task.Delay(500);
            switchInfo.State = SwitchState.On;
        }
        else if (switchId == "FOOBAZ_3")
        {
            await Task.Delay(200);
            switchInfo.State = SwitchState.Off;
        }

        return this.Ok(switchInfo);
    }
}
