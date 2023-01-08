using System.Net.Http.Json;
using Heimdall.Models.Dto;
using Heimdall.Models.Requests;
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

    private async Task CheckboxChangedAsync(ChangeEventArgs e)
    {
        var isChecked = (bool)e.Value;
        var newState = isChecked ? SwitchState.On : SwitchState.Off;

        try
        {
            var response = await this.Http.PostAsJsonAsync<SetSwitchStateRequest>(
                $"api/devices/switch/setstate/{this.Switch.Id}",
                new SetSwitchStateRequest
                {
                    State = newState,
                });
            response.EnsureSuccessStatusCode();
            this.Switch.State = newState;
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
    }
}
