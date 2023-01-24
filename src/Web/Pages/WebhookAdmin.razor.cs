using System.Net.Http.Json;
using Heimdall.Models;
using Heimdall.Models.Webhooks;
using Heimdall.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Heimdall.Web.Pages;

public partial class WebhookAdmin
{
    [Inject]
    private HttpClient Http { get; set; }

    [Inject]
    private BootstrapHelper BootstrapHelper { get; set; }

    private WebhookTable WebhookTable { get; set; }

    private FormModal<Webhook> NewWebhookModal { get; set; }

    private async Task CreateWebhookAsync(Webhook newDevice)
    {
        try
        {
            await this.Http.PostAsJsonAsync<Webhook>(
                $"api/admin/webhooks",
                newDevice,
                options: JsonHelpers.DefaultJsonOptions);
            await this.WebhookTable.ResetAsync();
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
    }

    private void AddWebhookAction()
    {
        this.NewWebhookModal.Model.Actions.Add(new ToggleSwitchAction
        {
            TargetDeviceId = "???",
        });
        this.StateHasChanged();
    }

    private Webhook CreateNewWebhookModel()
    {
        var webhook = new Webhook();
        webhook.Actions.Add(new ToggleSwitchAction
        {
            TargetDeviceId = "YayBaz",
        });
        return webhook;
    }
}
