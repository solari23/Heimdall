using System.Net.Http.Json;
using Heimdall.Models;
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
                $"api/devices/switches/{this.Switch.Id}",
                options: JsonHelpers.DefaultJsonOptions);
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
                $"api/devices/switches/{this.Switch.Id}/setstate",
                new SetSwitchStateRequest
                {
                    State = newState,
                },
                options: JsonHelpers.DefaultJsonOptions);
            response.EnsureSuccessStatusCode();
            this.Switch.State = newState;
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
    }
}
