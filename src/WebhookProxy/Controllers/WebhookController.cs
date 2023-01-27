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
        IMainStorageAccess mainStorage)
    {
        this.ActionProcessor = actionProcessor;
        this.MainStorage = mainStorage;
    }

    private ActionProcessor ActionProcessor { get; }

    private IMainStorageAccess MainStorage { get; }

    [HttpGet("{webhookId}")]
    public async Task<IActionResult> GetAsync(string webhookId)
    {
        var queryResult = await this.MainStorage.GetWebhookByIdAsync(webhookId);

        if (!queryResult.WasFound)
        {
            return this.NotFound();
        }

        var webhookDefinition = queryResult.Data;

        await this.ActionProcessor.ExecuteActionsAsync(webhookDefinition.Actions);
        return this.Ok($"You have called webhook {webhookId}");
    }
}
