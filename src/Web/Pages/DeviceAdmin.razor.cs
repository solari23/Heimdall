using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Heimdall.Web.Pages;

public partial class DeviceAdmin
{
    [Inject]
    private HttpClient Http { get; set; }

    // Placeholder
    public async Task DoThingAsync()
    {
        try
        {
            await this.Http.GetAsync("api/devices/admin/dothing");
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
    }
}
