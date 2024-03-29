﻿using System.Net.Http.Json;
using Heimdall.Models;
using Heimdall.Models.Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Heimdall.Web.Shared;

public partial class SwitchPanel
{
    [Inject]
    private HttpClient Http { get; set; }

    private List<SwitchInfo> switches;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            this.switches = await this.Http.GetFromJsonAsync<List<SwitchInfo>>(
                "api/devices/switches",
                options: JsonHelpers.DefaultJsonOptions);
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }

        await base.OnInitializedAsync();
    }
}
