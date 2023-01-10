// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

namespace Heimdall.Server.Storage;

public class QueryResult<TData>
    where TData : class
{
    public static QueryResult<TData> NotFound()
        => new QueryResult<TData>(false, null);

    public static QueryResult<TData> Found(TData data)
        => new QueryResult<TData>(true, data);

    private QueryResult(bool wasFound, TData data)
    {
        this.WasFound = wasFound;
        this.Data = data;
    }

    public bool WasFound { get; }

    public TData Data { get; }
}
