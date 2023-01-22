// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Text.Json;

using Heimdall.Models;
using Heimdall.Models.Webhooks;
using Microsoft.Data.Sqlite;

namespace Heimdall.CommonServices.Storage;

public partial class SqliteStorageAccess
{
    private const string AllWebhooksQuery = "SELECT Id, Name, Actions FROM Webhooks";

    private const string WebhookByIdQuery = $"SELECT Id, Name, Actions FROM Webhooks WHERE Id = ${nameof(Webhook.Id)}";

    private const string WebhookCreationCommand = $@"
        INSERT INTO Webhooks (Id, Name, Actions)
        VALUES (
            ${nameof(Webhook.Id)},
            ${nameof(Webhook.Name)},
            ${nameof(Webhook.Actions)})";

    private const string WebhookDeletionCommand = $"DELETE FROM Webhooks WHERE Id = ${nameof(Webhook.Id)}";

    public async Task<QueryResult<Webhook>> GetWebhookByIdAsync(string webhookId, CancellationToken ct = default)
    {
        var foundWebhooks = await this.ExecuteQueryAsync(
            WebhookByIdQuery,
            ReadWebhookObject,
            ct,
            (nameof(Webhook.Id), webhookId));

        if (!foundWebhooks.Any())
        {
            return QueryResult<Webhook>.NotFound();
        }

        return QueryResult<Webhook>.Found(foundWebhooks.First());
    }

    public async Task<QueryResult<List<Webhook>>> GetWebhooksAsync(CancellationToken ct = default)
    {
        var foundWebhooks = await this.ExecuteQueryAsync(
            AllWebhooksQuery,
            ReadWebhookObject,
            ct);

        return foundWebhooks.Any()
            ? QueryResult<List<Webhook>>.Found(foundWebhooks)
            : QueryResult<List<Webhook>>.NotFound();
    }

    public async Task AddWebhookAsync(Webhook webhook)
    {
        var command = this.Connection.Value.CreateCommand();
        command.CommandText = WebhookCreationCommand;

        command.Parameters.AddWithValue($"${nameof(Webhook.Id)}", webhook.Id);
        command.Parameters.AddWithValue($"${nameof(Webhook.Name)}", webhook.Name);

        var actionsJson = JsonSerializer.Serialize(
            webhook.Actions,
            JsonHelpers.DefaultJsonOptions);
        command.Parameters.AddWithValue($"${nameof(Webhook.Actions)}", actionsJson);

        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteWebhookAsync(string webhookId)
    {
        var command = this.Connection.Value.CreateCommand();
        command.CommandText = WebhookDeletionCommand;

        command.Parameters.AddWithValue($"${nameof(Webhook.Id)}", webhookId);

        await command.ExecuteNonQueryAsync();
    }

    private static Webhook ReadWebhookObject(SqliteDataReader reader)
        => new Webhook
        {
            Id = reader.GetString(0),
            Name = reader.GetString(1),
            Actions = JsonSerializer.Deserialize<List<IAction>>(reader.GetString(2), JsonHelpers.DefaultJsonOptions),
        };
}
