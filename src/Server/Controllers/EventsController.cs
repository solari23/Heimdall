// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.CommonServices.Storage;
using Heimdall.Models;
using Heimdall.Models.Events;
using Heimdall.Models.Requests;
using Heimdall.Server.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace Heimdall.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[RequiredScope(RequiredScopesConfigurationKey = AadHelpers.RequiredScopesConfigKey)]
[HeimdallRoleAuthorize(HeimdallRole.HomeAdmin)]
public class EventsController : Controller
{
    public EventsController(IEventStorageAccess eventStorageAccess)
    {
        this.EventStorageAccess = eventStorageAccess;
    }

    private IEventStorageAccess EventStorageAccess { get; }

    [HttpPost]
    public async Task<IActionResult> PublishAsync([FromBody] PublishEventRequest request)
    {
        if (request.Category == HeimdallEventCategory.Unknown)
        {
            return this.BadRequest();
        }

        var evt = new HeimdallEvent
        {
#if DEBUG
            TimeUtc = request.TimeOverrideForTesting ?? DateTimeOffset.UtcNow,
#else
            TimeUtc = DateTimeOffset.UtcNow,
#endif
            Category = request.Category,
            EventType = request.EventType,

            // TODO: Implement event message templating.
            Message = request.MessageTemplate,
        };

        await this.EventStorageAccess.AddEventAsync(evt);
        return this.Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync(
        [FromQuery] DateTimeOffset? since,
        [FromQuery] HeimdallEventCategory? category,
        [FromQuery] string eventType)
    {
        if (since is null)
        {
            since = DateTimeOffset.UtcNow - TimeSpan.FromDays(7);
        }

        var queryResult = await this.EventStorageAccess.QueryEventsAsync(
            since,
            category,
            eventType);

        var events = queryResult.WasFound
            ? queryResult.Data
            : new();
        return this.Ok(events);
    }
}
