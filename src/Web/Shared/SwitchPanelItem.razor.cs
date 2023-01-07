using System.Net.Http.Json;
using Heimdall.Models.Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Heimdall.Web.Shared;

public partial class SwitchPanelItem
{
    [Inject]
    private HttpClient Http { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            this.Switch = await this.Http.GetFromJsonAsync<SwitchInfo>(
                $"api/devices/switch/{this.Switch.Id}");
            this.LoadAttempted = true;
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }

        await base.OnInitializedAsync();
    }
}
