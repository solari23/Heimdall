using System.Net.Http.Json;
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
                "api/devices/switch/ListAll",
                options: Helpers.DefaultJsonOptions);
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }

        await base.OnInitializedAsync();
    }
}
