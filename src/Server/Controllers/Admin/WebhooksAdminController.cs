// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.CommonServices.Storage;
using Heimdall.Models;
using Heimdall.Models.Webhooks;
using Heimdall.Server.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Heimdall.Server.Controllers.Admin;

[Authorize]
[ApiController]
[Route("api/admin/webhooks")]
[RequiredScope(RequiredScopesConfigurationKey = AadHelpers.RequiredScopesConfigKey)]
[HeimdallRoleAuthorize(HeimdallRole.UberAdmin)]
public class WebhooksAdminController : Controller
{
    public WebhooksAdminController(IMainStorageAccess mainStorage)
    {
        this.MainStorage = mainStorage;
    }

    private IMainStorageAccess MainStorage { get; }

    [HttpGet]
    public async Task<IActionResult> ListAllAsync(CancellationToken ct)
    {
        var webhooksQueryResult = await this.MainStorage.GetWebhooksAsync(ct);

        var webhooks = webhooksQueryResult.WasFound
            ? webhooksQueryResult.Data
            : new List<Webhook>();

        return this.Ok(webhooks);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] Webhook newWebhookRequest)
    {
        // Ignore ID coming from client, which should be empty.
        // We'll generate a new unique ID.
        var newWebhook = newWebhookRequest with { Id = IdGenerator.CreateNewId() };

        await this.MainStorage.AddWebhookAsync(newWebhook);

        // Technically this should return HTTP 201 'Created' with a URI to the resource.
        // We aren't fully implementing REST resources here, though.
        return this.Ok(newWebhook);
    }

    [HttpPut("{webhookId}")]
    public async Task<IActionResult> UpdateAsync(string webhookId, [FromBody] Webhook webhookToUpdate)
    {
        if (webhookId != webhookToUpdate.Id)
        {
            return this.BadRequest();
        }

        bool rowUpdated = await this.MainStorage.UpdateWebhookAsync(webhookToUpdate);
        return rowUpdated ? this.Ok() : this.NotFound();
    }

    [HttpDelete("{webhookId}")]
    public async Task<IActionResult> DeleteAsync(string webhookId)
    {
        bool rowDeleted = await this.MainStorage.DeleteWebhookAsync(webhookId);
        return rowDeleted ? this.Ok() : this.NotFound();
    }
}
