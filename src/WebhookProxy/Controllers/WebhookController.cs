// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.CommonServices.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Heimdall.WebhookProxy.Controllers;

[ApiController]
[Route("[controller]")]
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
        return this.Ok($"You have called webhook {webhookId}");
    }
}
