// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace Heimdall.Server.Storage;

public class SqliteStorageAccess : IStorageAccess, IDisposable
{
    private const string TableCreationCommand = @"
        CREATE TABLE IF NOT EXISTS Devices (
            Id TEXT PRIMARY KEY,
            IPAddress TEXT NOT NULL,
            Type TEXT NOT NULL,
            Label TEXT NOT NULL
        ) WITHOUT ROWID;
    ";

    public SqliteStorageAccess(IOptions<SqliteStorageAccessOptions> options)
    {
        this.Options = options.Value;
        this.Connection = new Lazy<SqliteConnection>(() => this.OpenConnection());
    }

    private SqliteStorageAccessOptions Options { get; }

    private Lazy<SqliteConnection> Connection { get; }

    private SqliteConnection OpenConnection()
    {
        var connectionString = $"DataSource={this.Options.DatabaseFilePath}";
        var connection = new SqliteConnection(connectionString);
        connection.Open();

        var tableCreationCommand = connection.CreateCommand();
        tableCreationCommand.CommandText = TableCreationCommand;
        tableCreationCommand.ExecuteNonQuery();

        return connection;
    }

    // Placeholder
    public Task DoThingAsync()
    {
        var foo = this.Connection.Value.ServerVersion;
        foo.ToString();
        return Task.CompletedTask;
    }

    #region IDisposable Implementation

    private bool disposedValue;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                // Dispose managed state (managed objects)
                if (this.Connection.IsValueCreated)
                {
                    this.Connection.Value?.Dispose();
                }
            }

            // Free unmanaged resources (unmanaged objects) and override finalizer
            // (No unmanaged resources)
            this.disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
