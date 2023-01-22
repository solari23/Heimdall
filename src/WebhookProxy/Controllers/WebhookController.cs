// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.CommonServices.Storage;
using Heimdall.Models;
using Heimdall.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Heimdall.WebhookProxy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : Controller
{
    public WebhookController(
        IHttpClientFactory httpClientFactory,
        IStorageAccess storageAccess)
    {
        this.HttpClient = httpClientFactory.CreateClient(Program.HeimdallApiHttpClientName);
        this.StorageAccess = storageAccess;
    }

    private HttpClient HttpClient { get; }

    private IStorageAccess StorageAccess { get; }

    [HttpGet("{webhookId}")]
    public async Task<IActionResult> GetAsync(string webhookId)
    {
        await Task.Yield();
        return this.Ok();
    }

    [HttpGet]
    public async Task<IActionResult> TestAsync()
    {
        var switches = await this.HttpClient.GetFromJsonAsync<List<SwitchInfo>>(
            "api/devices/switches",
            JsonHelpers.DefaultJsonOptions);
        return this.Ok(switches.Count);
    }
}
