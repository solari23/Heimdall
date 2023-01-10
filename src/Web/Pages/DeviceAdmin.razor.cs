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

    private DeviceTable DeviceTable { get; set; }

    private FormModal<Device> NewDeviceModal { get; set; }

    public void ShowNewDeviceForm()
    {
        this.NewDeviceModal.Clear();
        this.NewDeviceModal.Open();
    }

    private async Task CreateDeviceAsync(Device newDevice)
    {
        try
        {
            await this.Http.PostAsJsonAsync<Device>(
                $"api/admin/devices",
                newDevice,
                options: Helpers.DefaultJsonOptions);
            await this.DeviceTable.ResetAsync();
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
    }
}
