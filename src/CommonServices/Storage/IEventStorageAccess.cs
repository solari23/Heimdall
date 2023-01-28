// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models.Events;

namespace Heimdall.CommonServices.Storage;

public interface IEventStorageAccess
{
    Task AddEventAsync(HeimdallEvent evt);

    Task<QueryResult<List<HeimdallEvent>>> QueryEventsAsync(
        DateTimeOffset? since = null,
        HeimdallEventCategory? category = null,
        string eventType = null,
        CancellationToken ct = default);

    Task<int> PurgeEventsAsync(DateTimeOffset? before);
}
