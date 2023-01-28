// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models.Events;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace Heimdall.CommonServices.Storage;

public class EventStorageAccess : SqliteStorageAccess, IEventStorageAccess
{
    private const string TableCreationCommandString = @"
        CREATE TABLE IF NOT EXISTS Events (
            Id INTEGER PRIMARY KEY,
            TimeUtc TEXT NOT NULL,
            Category TEXT NOT NULL,
            EventType TEXT NOT NULL,
            Message TEXT
        );";

    private const string InsertEventCommand = $@"
        INSERT INTO Events (TimeUtc, Category, EventType, Message)
        VALUES (
            ${nameof(HeimdallEvent.TimeUtc)},
            ${nameof(HeimdallEvent.Category)},
            ${nameof(HeimdallEvent.EventType)},
            ${nameof(HeimdallEvent.Message)})";

    public const string EventsQuery = @"
        SELECT TimeUtc, Category, EventType, Message
        FROM Events
        WHERE true";  // Makes it easier to append 'AND Foo = Bar' filter clauses later on.

    public const string PurgeEventsCommand = @"DELETE FROM Events";

    public EventStorageAccess(IOptionsMonitor<SqliteStorageAccessOptions> options)
        : base(options.Get(SqliteStorageAccessOptions.Instances.Event))
    {
        // Empty.
    }

    protected override string TableCreationCommand => TableCreationCommandString;

    public async Task AddEventAsync(HeimdallEvent evt)
    {
        var command = this.Connection.Value.CreateCommand();
        command.CommandText = InsertEventCommand;
        ParameterizeEvent(command.Parameters, evt);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<QueryResult<List<HeimdallEvent>>> QueryEventsAsync(
        DateTimeOffset? since = null,
        HeimdallEventCategory? category = null,
        string eventType = null,
        CancellationToken ct = default)
    {
        var query = EventsQuery;
        var parameters = new List<(string, object)>();

        if (since.HasValue)
        {
            query += $" AND TimeUtc > ${nameof(since)}";
            parameters.Add((nameof(since), since.Value));
        }

        if (category.HasValue)
        {
            query += $" AND Category = ${nameof(category)}";
            parameters.Add((nameof(category), category.Value.ToString()));
        }

        if (!string.IsNullOrEmpty(eventType))
        {
            query += $" AND Type = ${nameof(eventType)}";
            parameters.Add((nameof(eventType), eventType));
        }

        var events = await this.ExecuteQueryAsync(query, ReadEventObject, ct, parameters.ToArray());
        return events.Any()
            ? QueryResult<List<HeimdallEvent>>.Found(events)
            : QueryResult<List<HeimdallEvent>>.NotFound();
    }

    public async Task<int> PurgeEventsAsync(DateTimeOffset? before)
    {
        var command = this.Connection.Value.CreateCommand();
        command.CommandText = PurgeEventsCommand;

        if (before.HasValue)
        {
            command.CommandText += $" WHERE TimeUtc < ${nameof(before)}";
            command.Parameters.AddWithValue(nameof(before), before.Value);
        }

        return await command.ExecuteNonQueryAsync();
    }

    private static HeimdallEvent ReadEventObject(SqliteDataReader reader)
        => new HeimdallEvent
        {
            TimeUtc = reader.GetDateTimeOffset(0),
            Category = Enum.TryParse<HeimdallEventCategory>(reader.GetString(1), out var value)
                ? value
                : HeimdallEventCategory.Unknown,
            EventType = reader.GetString(2),
            Message = reader.GetString(3),
        };

    private static void ParameterizeEvent(SqliteParameterCollection parameters, HeimdallEvent evt)
    {
        parameters.AddWithValue($"${nameof(HeimdallEvent.TimeUtc)}", evt.TimeUtc);
        parameters.AddWithValue($"${nameof(HeimdallEvent.Category)}", evt.Category.ToString());
        parameters.AddWithValue($"${nameof(HeimdallEvent.EventType)}", evt.EventType);
        parameters.AddWithValue($"${nameof(HeimdallEvent.Message)}", evt.Message);
    }
}
