using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Heimdall.Web.Pages;

public partial class Counter
{
    [Inject]
    private HttpClient Http { get; set; }

    private int currentCount = 0;

    private void IncrementCount()
    {
        this.currentCount++;
    }

    public async Task DoAdminApiCallAsync()
    {
        try
        {
            await this.Http.GetAsync("api/Test/AdminNeeded");
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
    }

    public async Task DoViewerApiCallAsync()
    {
        try
        {
            await this.Http.GetAsync("api/Test/ViewerNeeded");
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
    }
}
