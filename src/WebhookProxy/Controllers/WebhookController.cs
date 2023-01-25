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
        ActionProcessor actionProcessor,
        IStorageAccess storageAccess)
    {
        this.ActionProcessor = actionProcessor;
        this.StorageAccess = storageAccess;
    }

    private ActionProcessor ActionProcessor { get; }

    private IStorageAccess StorageAccess { get; }

    [HttpGet("{webhookId}")]
    public async Task<IActionResult> GetAsync(string webhookId)
    {
        var queryResult = await this.StorageAccess.GetWebhookByIdAsync(webhookId);

        if (!queryResult.WasFound)
        {
            return this.NotFound();
        }

        var webhookDefinition = queryResult.Data;

        await this.ActionProcessor.ExecuteActionsAsync(webhookDefinition.Actions);
        return this.Ok($"You have called webhook {webhookId}");
    }
}
