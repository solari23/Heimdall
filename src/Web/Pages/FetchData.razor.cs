using System.Net.Http.Json;
using Heimdall.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Heimdall.Web.Pages;

public partial class FetchData
{
    [Inject]
    private HttpClient Http { get; set; }

    private WeatherForecast[] forecasts;
    protected override async Task OnInitializedAsync()
    {
        try
        {
            this.forecasts = await this.Http.GetFromJsonAsync<WeatherForecast[]>(
                "api/WeatherForecast",
                options: Helpers.DefaultJsonOptions);
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
    }
}
