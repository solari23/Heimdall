using System.Net.Http.Json;
using Heimdall.Models;
using Heimdall.Models.Webhooks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.JSInterop;

namespace Heimdall.Web.Shared;

public partial class WebhookTable
{
    [Inject]
    private HttpClient Http { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; }

    private List<Webhook> webhooks;

    public async Task ResetAsync()
    {
        try
        {
            this.webhooks = null;
            this.StateHasChanged();

            this.webhooks = await this.Http.GetFromJsonAsync<List<Webhook>>(
                "api/admin/webhooks",
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

    private async Task CopyWebhookUriToClipboardAsync(Webhook webhook)
    {
        // TODO: Form webhook URL for clipboard.
        await this.JSRuntime.CopyToClipboardAsync("HELLO WORLD");
    }

    private async Task ConfirmAndDeleteWebhookAsync(Webhook webhookToDelete)
    {
        var promptString = $"Are you sure you want to delete webhook '{webhookToDelete.Name}'?";
        if (!await this.JSRuntime.ConfirmAsync(promptString))
        {
            // Confirmation cancelled.
            return;
        }

        try
        {
            var response = await this.Http.DeleteAsync($"api/admin/webhooks/{webhookToDelete.Id}");
            response.EnsureSuccessStatusCode();
            this.webhooks.RemoveAll(w => w.Id == webhookToDelete.Id);
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
    }

    private async Task<string> GetActionDisplayStringAsync(IAction action)
    {
        if (action is ToggleSwitchAction toggleSwitchAction)
        {
            // TODO: Resolve the device ID to friendly name.
            return $"Toggle switch '{toggleSwitchAction.TargetDeviceId}'";
        }
        else
        {
            return EnumUtil<ActionKind>.ToPrettyString(action.ActionKind);
        }
    }
}
