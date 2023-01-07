using Heimdall.Models.Dto;
using Microsoft.AspNetCore.Components;

namespace Heimdall.Web.Shared;

public partial class SwitchPanelItem
{
    [Inject]
    private HttpClient Http { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var newState = SwitchState.Unknown;

        if (this.Switch.State == SwitchState.Unknown)
        {
            // TODO: Make call.
            await Task.Delay(1000);

            if (this.Switch.Id == "FOOBAZ_2")
            {
                await Task.Delay(500);
                newState = SwitchState.On;
            }
            else if (this.Switch.Id == "FOOBAZ_3")
            {
                await Task.Delay(200);
                newState = SwitchState.Off;
            }
        }

        this.Switch.State = newState;
        this.LoadAttempted = true;

        await base.OnInitializedAsync();
    }
}
