// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models;
using Heimdall.Models.Requests;
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
            else if (action is SetSwitchStateAction setSwitchStateAction)
            {
                tasks.Add(this.ExecuteSetSwitchStateActionAsync(setSwitchStateAction));
            }
            else if (action is PublishEventAction publishEventAction)
            {
                tasks.Add(this.ExecutePublishEventActionAsync(publishEventAction));
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

    private async Task ExecuteSetSwitchStateActionAsync(SetSwitchStateAction action)
    {
        await this.ApiClient.PostAsJsonAsync<SetSwitchStateRequest>(
            $"api/devices/switches/{action.TargetDeviceId}/setstate",
            new SetSwitchStateRequest
            {
                State = action.State,
            },
            options: JsonHelpers.DefaultJsonOptions);
    }

    private async Task ExecutePublishEventActionAsync(PublishEventAction action)
    {
        await this.ApiClient.PostAsJsonAsync<PublishEventRequest>(
            "api/events",
            new PublishEventRequest
            {
                Category = action.Category,
                EventType = action.EventType,
                MessageTemplate = action.MessageTemplate,

                // TODO: Pass along message template parameters to the backend API.
                //       Need to extract parameters from the MessageTemplate and
                //       check what headers/QS parameters to send along.
                TemplateParameters = new Dictionary<string, string>(),
            },
            options: JsonHelpers.DefaultJsonOptions);
    }
}
