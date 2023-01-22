// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Text.Json;
using Heimdall.CommonServices.Storage;
using Heimdall.Models;
using Heimdall.Models.Dto;
using Heimdall.Models.Webhooks;
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
        var webhook = new Webhook
        {
            Id = "FooId",
            Name = "BarName",
            Actions = new()
            {
                new ToggleSwitchAction
                {
                    TargetDeviceId = "BazTargetId",
                },
            },
        };

        var foo = JsonSerializer.Serialize(webhook, JsonHelpers.DefaultJsonOptions);
        var bar = JsonSerializer.Deserialize<Webhook>(foo, JsonHelpers.DefaultJsonOptions);

        return this.Ok(bar);
    }
}
