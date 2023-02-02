// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.CommonServices.Storage;
using Heimdall.Integrations;
using Heimdall.Models;
using Heimdall.Models.Dto;
using Heimdall.Models.Requests;
using Heimdall.Server.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Heimdall.Server.Controllers.Devices;

[Authorize]
[ApiController]
[Route("api/devices/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = AadHelpers.RequiredScopesConfigKey)]
public class SwitchesController : Controller
{
    public SwitchesController(
        IDeviceControllerFactory deviceControllerFactory,
        IMainStorageAccess mainStorage)
    {
        this.DeviceControllerFactory = deviceControllerFactory;
        this.MainStorage = mainStorage;
    }

    private IDeviceControllerFactory DeviceControllerFactory { get; }

    private IMainStorageAccess MainStorage { get; }

    [HttpGet]
    [HeimdallRoleAuthorize(HeimdallRole.HomeViewer)]
    public async Task<IActionResult> ListAllAsync()
    {
        var queryResult = await this.MainStorage.GetDevicesAsync(
            DeviceType.ShellyPlug,
            DeviceType.TasmotaPlug);

        var switches = queryResult.WasFound
            ? queryResult.Data.Select(d =>
                new SwitchInfo
                {
                    Id = d.Id,
                    Label = d.Name,
                    State = SwitchState.Unknown,
                }).ToArray()
            : Array.Empty<SwitchInfo>();

        return this.Ok(switches);
    }

    [HttpGet("{switchId}")]
    [HeimdallRoleAuthorize(HeimdallRole.HomeViewer)]
    public async Task<IActionResult> GetAsync(string switchId)
    {
        var queryResult = await this.MainStorage.GetDeviceByIdAsync(switchId);

        if (!queryResult.WasFound)
        {
            return this.NotFound();
        }

        var switchState = SwitchState.Unknown;

        try
        {
            var switchController = this.DeviceControllerFactory.GetSwitchController(queryResult.Data);
            switchState = await switchController.GetCurrentStateAsync();
        }
        catch (Exception)
        {
            // Swallow.
        }

        var switchInfo = new SwitchInfo
        {
            Id = queryResult.Data.Id,
            Label = queryResult.Data.Name,
            State = switchState,
        };

        return this.Ok(switchInfo);
    }

    [HttpPost("{switchId}/SetState")]
    [HeimdallRoleAuthorize(HeimdallRole.HomeAdmin)]
    public async Task<IActionResult> SetStateAsync(
        string switchId,
        [FromBody]SetSwitchStateRequest request)
    {
        var queryResult = await this.MainStorage.GetDeviceByIdAsync(switchId);

        if (!queryResult.WasFound)
        {
            return this.NotFound();
        }

        var switchController = this.DeviceControllerFactory.GetSwitchController(queryResult.Data);

        switch (request.State)
        {
            case SwitchState.On:
                await switchController.TurnOnAsync();
                break;

            case SwitchState.Off:
                await switchController.TurnOffAsync();
                break;

            default:
                return this.BadRequest();
        }

        return this.Ok();
    }

    [HttpGet("{switchId}/Toggle")]
    [HeimdallRoleAuthorize(HeimdallRole.HomeAdmin)]
    public async Task<IActionResult> ToggleAsync(string switchId)
    {
        var queryResult = await this.MainStorage.GetDeviceByIdAsync(switchId);

        if (!queryResult.WasFound)
        {
            return this.NotFound();
        }

        var switchController = this.DeviceControllerFactory.GetSwitchController(queryResult.Data);
        await switchController.ToggleAsync();

        return this.Ok();
    }
}
