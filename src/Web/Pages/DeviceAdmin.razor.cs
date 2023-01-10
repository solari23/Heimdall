using System.Net.Http.Json;
using Heimdall.Models;
using Heimdall.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Heimdall.Web.Pages;

public partial class DeviceAdmin
{
    [Inject]
    private HttpClient Http { get; set; }

    private FormModal<Device> NewDeviceModal { get; set; }

    // Placeholder
    public async Task ListDevicesAsync()
    {
        this.NewDeviceModal.Open();
        await Task.Yield();
    }

    private async Task CreateDeviceAsync(Device newDevice)
    {
        try
        {
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
