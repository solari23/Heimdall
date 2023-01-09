using System.Net.Http.Json;
using Heimdall.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Heimdall.Web.Pages;

public partial class DeviceAdmin
{
    [Inject]
    private HttpClient Http { get; set; }

    // Placeholder
    public async Task ListDevicesAsync()
    {
        try
        {
            var devices = await this.Http.GetFromJsonAsync<List<Device>>(
                "api/admin/devices",
                options: Helpers.DefaultJsonOptions);
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
    }

    public async Task CreateDeviceAsync()
    {
        try
        {
            var newDevice = new Device
            {
                Type = DeviceType.ShellySwitch,
                Name = "Fake Device " + Guid.NewGuid().ToString().Split('-')[0],
                HostOrIPAddress = "127.0.0.1",
            };

            await this.Http.PostAsJsonAsync<Device>(
                $"api/admin/devices",
                newDevice,
                options: Helpers.DefaultJsonOptions);
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
    }
}
