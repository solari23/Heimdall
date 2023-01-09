// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace Heimdall.Server.Storage;

public class SqliteStorageAccess : IStorageAccess, IDisposable
{
    private const string TableCreationCommand = @"
        CREATE TABLE IF NOT EXISTS Devices (
            Id TEXT PRIMARY KEY,
            Type TEXT NOT NULL,
            Name TEXT NOT NULL,
            HostOrIPAddress TEXT NOT NULL
        ) WITHOUT ROWID;
    ";

    private const string AllDevicesQuery = "SELECT Id, Type, Name, HostOrIPAddress FROM Devices";

    private const string DeviceCreationCommand = $@"
        INSERT INTO Devices (Id, Type, Name, HostOrIPAddress)
        VALUES (
            ${nameof(Device.Id)},
            ${nameof(Device.Type)},
            ${nameof(Device.Name)},
            ${nameof(Device.HostOrIPAddress)})";

    private const string DeviceDeletionCommand = $@"DELETE FROM Devices WHERE Id = ${nameof(Device.Id)}";

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
    public async Task DoThingAsync()
    {
        var foo = this.Connection.Value.ServerVersion;
        foo.ToString();

        var bar = await this.GetDevicesAsync(DeviceType.ShellySwitch, DeviceType.Unknown);

        // return Task.CompletedTask;
    }

    public Task<List<Device>> GetDevicesAsync(params DeviceType[] typeFilter)
        => this.GetDevicesAsync(default, typeFilter);

    public async Task<List<Device>> GetDevicesAsync(CancellationToken ct, params DeviceType[] typeFilter)
    {
        string query = AllDevicesQuery;

        if (typeFilter is not null && typeFilter.Length > 0)
        {
            var allowedTypesExpression = string.Join(
                ',',
                typeFilter.Select(t => $"'{t}'"));

            // No injection possible -- the string is formed from fully controlled enum values.
            query += $" WHERE Type IN ({allowedTypesExpression})";
        }

        var devices = new List<Device>();

        var command = this.Connection.Value.CreateCommand();
        command.CommandText = query;

        using var queryReader = await command.ExecuteReaderAsync(ct);

        while (queryReader.Read())
        {
            var device = new Device
            {
                Id = queryReader.GetString(0),
                Type = Enum.TryParse<DeviceType>(queryReader.GetString(1), out var typeValue) 
                    ? typeValue
                    : DeviceType.Unknown,
                Name = queryReader.GetString(2),
                HostOrIPAddress = queryReader.GetString(3),
            };

            devices.Add(device);
        }

        return devices;
    }

    public async Task DeleteDeviceAsync(string deviceId, CancellationToken ct = default)
    {
        var command = this.Connection.Value.CreateCommand();
        command.CommandText = DeviceDeletionCommand;

        command.Parameters.AddWithValue($"${nameof(Device.Id)}", deviceId);

        await command.ExecuteNonQueryAsync();
    }

    public async Task AddDeviceAsync(Device device, CancellationToken ct = default)
    {
        var command = this.Connection.Value.CreateCommand();
        command.CommandText = DeviceCreationCommand;

        command.Parameters.AddWithValue($"${nameof(Device.Id)}", device.Id);
        command.Parameters.AddWithValue($"${nameof(Device.Type)}", device.Type.ToString());
        command.Parameters.AddWithValue($"${nameof(Device.Name)}", device.Name);
        command.Parameters.AddWithValue($"${nameof(Device.HostOrIPAddress)}", device.HostOrIPAddress);

        await command.ExecuteNonQueryAsync();
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
