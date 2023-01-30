// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Net.Http.Json;

using Heimdall.Models;
using Heimdall.Models.Events;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.WebUtilities;

namespace Heimdall.Web.Shared;

public partial class EventView
{
    [Parameter]
    public string Title { get; set; }

    [Parameter]
    public HeimdallEventCategory? CategoryFilter { get; set; }

    [Parameter]
    public TimeRange TimeRange { get; set; }

    [Inject]
    private HttpClient ApiClient { get; set; }

    private IQueryable<HeimdallEvent> Events { get; set; }

    public async Task ReloadAsync()
    {
        try
        {
            this.Events = null;
            this.StateHasChanged();

            var apiPath = this.GetEventsApiPathWithQuery();
            var events = await this.ApiClient.GetFromJsonAsync<List<HeimdallEvent>>(
                apiPath,
                options: JsonHelpers.DefaultJsonOptions);

            // Adjust timezones for local display.
            foreach (var evt in events)
            {
                evt.TimeUtc = evt.TimeUtc.ChangeTimezone();
            }

            this.Events = events.AsQueryable();
            this.StateHasChanged();
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await this.ReloadAsync();
        await base.OnInitializedAsync();
    }

    private string GetEventsApiPathWithQuery()
    {
        var queryParams = new Dictionary<string, string>
        {
            { "since", TimeUtil.GetStartTimeForTimeRange(this.TimeRange).ToString("O") },
        };

        if (this.CategoryFilter.HasValue)
        {
            queryParams.Add("category", this.CategoryFilter.Value.ToString());
        }

        var pathAndQuery = QueryHelpers.AddQueryString("api/events", queryParams);
        return pathAndQuery;
    }
}
