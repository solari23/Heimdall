// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Microsoft.Data.Sqlite;

namespace Heimdall.CommonServices.Storage;

public abstract class SqliteStorageAccess : IDisposable
{
    public SqliteStorageAccess(SqliteStorageAccessOptions options)
    {
        this.Options = options;
        this.Connection = new Lazy<SqliteConnection>(() => this.OpenConnection());
    }

    protected abstract string TableCreationCommand { get; }

    protected SqliteStorageAccessOptions Options { get; }

    protected Lazy<SqliteConnection> Connection { get; }

    protected SqliteConnection OpenConnection()
    {
        var connectionStringBuilder = new SqliteConnectionStringBuilder();
        connectionStringBuilder.DataSource = this.Options.DatabaseFilePath;
        connectionStringBuilder.Mode = this.Options.ReadOnly
            ? SqliteOpenMode.ReadOnly
            : SqliteOpenMode.ReadWriteCreate;

        var connection = new SqliteConnection(connectionStringBuilder.ConnectionString);
        connection.Open();

        var tableCreationCommand = connection.CreateCommand();
        tableCreationCommand.CommandText = this.TableCreationCommand;
        tableCreationCommand.ExecuteNonQuery();

        return connection;
    }

    protected async Task<List<T>> ExecuteQueryAsync<T>(
        string query,
        Func<SqliteDataReader, T> objectReader,
        CancellationToken ct,
        params (string Key, object Value)[] parameters)
    {
        var objects = new List<T>();

        var command = this.Connection.Value.CreateCommand();
        command.CommandText = query;

        if (parameters is not null)
        {
            foreach (var parameter in parameters)
            {
                command.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }
        }

        using var queryReader = await command.ExecuteReaderAsync(ct);

        while (queryReader.Read())
        {
            var newObject = objectReader(queryReader);
            objects.Add(newObject);
        }

        return objects;
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
