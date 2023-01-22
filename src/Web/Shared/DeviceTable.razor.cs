using System.Net.Http.Json;
using Heimdall.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.JSInterop;

namespace Heimdall.Web.Shared;

public partial class DeviceTable
{
    [Inject]
    private HttpClient Http { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; }

    private List<Device> devices;

    public async Task ResetAsync()
    {
        try
        {
            this.devices = null;
            this.StateHasChanged();

            this.devices = await this.Http.GetFromJsonAsync<List<Device>>(
                "api/admin/devices",
                options: JsonHelpers.DefaultJsonOptions);
            this.StateHasChanged();
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
    }

    protected async override Task OnInitializedAsync()
    {
        await this.ResetAsync();
    }

    private async Task ConfirmAndDeleteDeviceAsync(Device deviceToDelete)
    {
        var promptString = $"Are you sure you want to delete {EnumUtil<DeviceType>.ToPrettyString(deviceToDelete.Type)} '{deviceToDelete.Name}'?";
        if (!await this.JSRuntime.ConfirmAsync(promptString))
        {
            // Confirmation cancelled.
            return;
        }

        try
        {
            var response = await this.Http.DeleteAsync($"api/admin/devices/{deviceToDelete.Id}");
            response.EnsureSuccessStatusCode();
            this.devices.RemoveAll(d => d.Id == deviceToDelete.Id);
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
            DeviceType.ShellyPlug => "oi-pulse",
            _ => "oi-question-mark",
        };
}
