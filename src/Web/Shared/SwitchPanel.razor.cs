using Heimdall.Models.Dto;
using Microsoft.AspNetCore.Components;

namespace Heimdall.Web.Shared;

public partial class SwitchPanel
{
    [Inject]
    private HttpClient Http { get; set; }

    private List<SwitchInfo> switches;

    protected override async Task OnInitializedAsync()
    {
        await Task.Delay(1500);
        this.switches = new List<SwitchInfo>
        {
            new SwitchInfo
            {
                Id = "FOOBAZ_1",
                Label = "The best switch ever",
                State = SwitchState.Unknown,
            },
            new SwitchInfo
            {
                Id = "FOOBAZ_2",
                Label = "This one is on",
                State = SwitchState.Unknown,
            },
            new SwitchInfo
            {
                Id = "FOOBAZ_3",
                Label = "This one is off",
                State = SwitchState.Unknown,
            },
        };

        await base.OnInitializedAsync();
    }
}
