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

    private void AddWebhookAction(ActionKind actionKind)
    {
        var newAction = actionKind switch
        {
            ActionKind.ToggleSwitch => new ToggleSwitchAction(),
            _ => throw new NotSupportedException(
                $"Don't know how to render UI for action kind '{actionKind}'"),
        };

        this.NewWebhookModal.Model.Actions.Add(newAction);
        this.StateHasChanged();
    }

    private async Task DeleteWebhookActionAsync(int index)
    {
        this.NewWebhookModal.Model.Actions.RemoveAt(index);
        await this.BootstrapHelper.ReleaseCollapseAsync(
            ModalCollapseGroupTag,
            ModalCollapseElementIdForIndex(index));

        this.StateHasChanged();
    }
}
