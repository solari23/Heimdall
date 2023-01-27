using System.Net.Http.Json;
using Heimdall.Models;
using Heimdall.Models.Webhooks;
using Heimdall.Web.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Heimdall.Web.Pages;

public partial class WebhookAdmin
{
    private const string ModalCollapseGroupTag = "WH_ADMIN_MODAL_GRP";
    private static string ModalCollapseElementIdForIndex(int index) => $"webhookActions_collapse_{index}";

    [Inject]
    private HttpClient Http { get; set; }

    [Inject]
    private BootstrapHelper BootstrapHelper { get; set; }

    private WebhookTable WebhookTable { get; set; }

    private FormModal<Webhook> WebhookEditModal { get; set; }

    private async Task OnWebhookFormModalCloseAsync()
    {
        await this.BootstrapHelper.ReleaseCollapseGroupAsync(ModalCollapseGroupTag);
        await this.WebhookTable.ResetAsync();
    }

    private async Task CreateOrUpdateWebhookAsync(Webhook webhook)
    {
        try
        {
            if (webhook.Id is null)
            {
                // Create it.
                await this.Http.PostAsJsonAsync<Webhook>(
                    $"api/admin/webhooks",
                    webhook,
                    options: JsonHelpers.DefaultJsonOptions);
            }
            else
            {
                // Update it.
                await this.Http.PutAsJsonAsync<Webhook>(
                    $"api/admin/webhooks/{webhook.Id}",
                    webhook,
                    options: JsonHelpers.DefaultJsonOptions);
            }
        }
        catch (AccessTokenNotAvailableException exception)
        {
            exception.Redirect();
        }
    }

    private void AddWebhookAction(ActionKind actionKind)
    {
        var newAction = actionKind switch
        {
            ActionKind.ToggleSwitch => new ToggleSwitchAction(),
            _ => throw new NotSupportedException(
                $"Don't know how to render UI for action kind '{actionKind}'"),
        };

        this.WebhookEditModal.Model.Actions.Add(newAction);
        this.StateHasChanged();
    }

    private async Task DeleteWebhookActionAsync(int index)
    {
        this.WebhookEditModal.Model.Actions.RemoveAt(index);
        await this.BootstrapHelper.ReleaseCollapseAsync(
            ModalCollapseGroupTag,
            ModalCollapseElementIdForIndex(index));

        this.StateHasChanged();
    }
}
