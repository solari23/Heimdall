// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models;
using Heimdall.Models.Dto;
using Heimdall.Models.Requests;
using Heimdall.Server.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Heimdall.Server.Controllers.Devices;

[ApiController]
[Route("api/devices/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class SwitchesController : Controller
{
    private static List<SwitchInfo> Switches => new()
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

    private static readonly Dictionary<string, SwitchState> CurrentStates = new()
    {
        { "FOOBAZ_1", SwitchState.Unknown },
        { "FOOBAZ_2", SwitchState.On },
        { "FOOBAZ_3", SwitchState.Off },
    };

    [HttpGet]
    [HeimdallRoleAuthorize(HeimdallRole.HomeViewer)]
    public async Task<IActionResult> ListAllAsync()
    {
        await Task.Delay(700);
        return this.Ok(Switches);
    }

    [HttpGet("{switchId}")]
    [HeimdallRoleAuthorize(HeimdallRole.HomeViewer)]
    public async Task<IActionResult> GetAsync(string switchId)
    {
        await Task.Delay(400);
        if (switchId == "FOOBAZ_2")
        {
            await Task.Delay(500);
        }
        else if (switchId == "FOOBAZ_3")
        {
            await Task.Delay(200);
        }

        var switchInfo = Switches.FirstOrDefault(s => s.Id == switchId);

        if (switchInfo is null
            || !CurrentStates.TryGetValue(switchId, out var currentState))
        {
            return this.NotFound();
        }

        switchInfo.State = currentState;

        return this.Ok(switchInfo);
    }

    [HttpPost("{switchId}/SetState")]
    [HeimdallRoleAuthorize(HeimdallRole.HomeAdmin)]
    public async Task<IActionResult> SetStateAsync(
        string switchId,
        [FromBody]SetSwitchStateRequest request)
    {
        await Task.Delay(200);

        if (request.State == SwitchState.Unknown)
        {
            // TODO: Define and send standard error response.
            return this.BadRequest();
        }

        if (!CurrentStates.ContainsKey(switchId))
        {
            return this.NotFound();
        }

        CurrentStates[switchId] = request.State;
        return this.Ok();
    }
}
