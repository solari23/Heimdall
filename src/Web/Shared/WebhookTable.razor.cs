// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Net.Http.Json;

using Heimdall.Models;
using Heimdall.Models.Webhooks;
using Heimdall.Web.Services;
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

    [Inject]
    private IConfiguration Config { get; set; }

    [Inject]
    private DeviceRepository DeviceRepository { get; set; }

    [Parameter]
    public Action<Webhook> EditUIDelegate { get; set; }

    private List<Webhook> webhooks;

    private IReadOnlyDictionary<string, string> deviceIdToNameMapping = new Dictionary<string, string>();

    private bool allowRendering = true;

    public async Task ResetAsync()
    {
        try
        {
            this.allowRendering = true;
            this.deviceIdToNameMapping
                = await this.DeviceRepository.GetDeviceIdToNameMappingAsync();

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

    protected override bool ShouldRender() => this.allowRendering;

    private void ShowEditUI(Webhook webhook)
    {
        // Pause rendering the table.
        // Caller's responsibility to re-enable via Reset when editing is done.
        this.allowRendering = false;
        this.EditUIDelegate(webhook);
    }

    private async Task CopyWebhookUriToClipboardAsync(Webhook webhook)
    {
        var template = this.Config.GetValue<string>("HeimdallWebhookTemplate");
        var uri = string.IsNullOrWhiteSpace(template)
            ? webhook.Id
            : string.Format(template, webhook.Id);
        await this.JSRuntime.CopyToClipboardAsync(uri);
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

    private string GetActionDisplayString(IAction action)
    {
        if (action is ToggleSwitchAction toggleSwitchAction)
        {
            if (!this.deviceIdToNameMapping.TryGetValue(
                toggleSwitchAction.TargetDeviceId,
                out var targetDeviceFriendlyName))
            {
                targetDeviceFriendlyName = toggleSwitchAction.TargetDeviceId;
            }

            return $"Toggle switch '{targetDeviceFriendlyName}'";
        }
        else
        {
            return EnumUtil<ActionKind>.ToPrettyString(action.ActionKind);
        }
    }
}
