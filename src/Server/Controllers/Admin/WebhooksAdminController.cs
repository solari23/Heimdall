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
    public WebhooksAdminController(IStorageAccess storageAccess)
    {
        this.StorageAccess = storageAccess;
    }

    private IStorageAccess StorageAccess { get; }

    [HttpGet]
    public async Task<IActionResult> ListAllAsync(CancellationToken ct)
    {
        var webhooksQueryResult = await this.StorageAccess.GetWebhooksAsync(ct);

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

        await this.StorageAccess.AddWebhookAsync(newWebhook);

        // Technically this should return HTTP 201 'Created' with a URI to the resource.
        // We aren't fully implementing REST resources here, though.
        return this.Ok(newWebhook);
    }

    [HttpDelete("{webhookId}")]
    public async Task<IActionResult> DeleteAsync(string webhookId)
    {
        await this.StorageAccess.DeleteWebhookAsync(webhookId);
        return this.Ok();
    }
}
