// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Microsoft.Extensions.Options;

namespace Heimdall.CommonServices.Storage;

public partial class MainStorageAccess : SqliteStorageAccess, IMainStorageAccess
{
    private const string TableCreationCommandString = @"
        CREATE TABLE IF NOT EXISTS Devices (
            Id TEXT PRIMARY KEY,
            Type TEXT NOT NULL,
            Name TEXT NOT NULL,
            HostOrIPAddress TEXT NOT NULL
        ) WITHOUT ROWID;

        CREATE TABLE IF NOT EXISTS Webhooks (
            Id TEXT PRIMARY KEY,
            Name TEXT NOT NULL,
            Actions TEXT NOT NULL
        ) WITHOUT ROWID;
    ";

    public MainStorageAccess(IOptionsMonitor<SqliteStorageAccessOptions> options)
        : base(options.Get(SqliteStorageAccessOptions.Instances.Main))
    {
        // Empty.
    }

    protected override string TableCreationCommand => TableCreationCommandString;
}
