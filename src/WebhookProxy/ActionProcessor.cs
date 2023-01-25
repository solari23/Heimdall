// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models.Webhooks;

namespace Heimdall.WebhookProxy;

public class ActionProcessor
{
    public ActionProcessor(
        IHttpClientFactory httpClientFactory)
    {
        this.ApiClient = httpClientFactory.CreateClient(Program.HeimdallApiHttpClientName);
    }

    private HttpClient ApiClient { get; }

    public async Task ExecuteActionsAsync(IReadOnlyList<IAction> actions)
    {
        if (actions is null || actions.Count == 0)
        {
            // Nothing to do.
            return;
        }

        List<Task> tasks = new List<Task>();

        foreach (var action in actions)
        {
            if (action is ToggleSwitchAction toggleSwitchAction)
            {
                tasks.Add(this.ExecuteToggleSwitchActionAsync(toggleSwitchAction));
            }
            else
            {
                throw new NotImplementedException($"Action of type '{action.GetType().Name}' is not implemented");
            }
        }

        await Task.WhenAll(tasks);
    }

    private async Task ExecuteToggleSwitchActionAsync(ToggleSwitchAction action)
    {
        await this.ApiClient.GetAsync($"api/devices/switches/{action.TargetDeviceId}/toggle");
    }
}
