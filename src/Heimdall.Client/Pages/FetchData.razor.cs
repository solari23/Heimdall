using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Heimdall.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Authorization;

namespace Heimdall.Client.Pages;

[Authorize]
public partial class FetchData
{
    [Inject]
    private HttpClient Http { get; set; }

    private WeatherForecast[] forecasts;
    protected override async Task OnInitializedAsync()
    {
        try
        {
            forecasts = await Http.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast");
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
    }
}
