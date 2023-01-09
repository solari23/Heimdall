using System.Net.Http.Json;
using Heimdall.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Heimdall.Web.Shared;

public partial class DeviceTable
{
    [Inject]
    private HttpClient Http { get; set; }

    private List<Device> devices;

    protected async override Task OnInitializedAsync()
    {
        try
        {
            this.devices = await this.Http.GetFromJsonAsync<List<Device>>(
                "api/admin/devices",
                options: Helpers.DefaultJsonOptions);
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
    }

    private static string DeviceTypeToIconClass(DeviceType type)
        => type switch
        {
            DeviceType.Unknown => "oi-question-mark",
            DeviceType.ShellySwitch => "oi-pulse",
            _ => "oi-question-mark",
        };
}
