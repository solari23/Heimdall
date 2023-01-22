// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models;
using Microsoft.Data.Sqlite;

namespace Heimdall.CommonServices.Storage;

public partial class SqliteStorageAccess
{
    private const string AllDevicesQuery = "SELECT Id, Type, Name, HostOrIPAddress FROM Devices";

    private const string DeviceByIdQuery = $"SELECT Id, Type, Name, HostOrIPAddress FROM Devices WHERE Id = ${nameof(Device.Id)}";

    private const string DeviceCreationCommand = $@"
        INSERT INTO Devices (Id, Type, Name, HostOrIPAddress)
        VALUES (
            ${nameof(Device.Id)},
            ${nameof(Device.Type)},
            ${nameof(Device.Name)},
            ${nameof(Device.HostOrIPAddress)})";

    private const string DeviceDeletionCommand = $"DELETE FROM Devices WHERE Id = ${nameof(Device.Id)}";

    public async Task<QueryResult<Device>> GetDeviceByIdAsync(string deviceId, CancellationToken ct = default)
    {
        var foundDevices = await this.ExecuteQueryAsync(
            DeviceByIdQuery,
            ReadDeviceObject,
            ct,
            (nameof(Device.Id), deviceId));

        if (!foundDevices.Any())
        {
            return QueryResult<Device>.NotFound();
        }

        return QueryResult<Device>.Found(foundDevices.First());
    }

    public Task<QueryResult<List<Device>>> GetDevicesAsync(params DeviceType[] typeFilter)
        => this.GetDevicesAsync(default, typeFilter);

    public async Task<QueryResult<List<Device>>> GetDevicesAsync(CancellationToken ct, params DeviceType[] typeFilter)
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

        var foundDevices = await this.ExecuteQueryAsync(query, ReadDeviceObject, ct);
        return foundDevices.Any()
            ? QueryResult<List<Device>>.Found(foundDevices)
            : QueryResult<List<Device>>.NotFound();
    }

    public async Task AddDeviceAsync(Device device)
    {
        var command = this.Connection.Value.CreateCommand();
        command.CommandText = DeviceCreationCommand;

        command.Parameters.AddWithValue($"${nameof(Device.Id)}", device.Id);
        command.Parameters.AddWithValue($"${nameof(Device.Type)}", device.Type.ToString());
        command.Parameters.AddWithValue($"${nameof(Device.Name)}", device.Name);
        command.Parameters.AddWithValue($"${nameof(Device.HostOrIPAddress)}", device.HostOrIPAddress);

        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteDeviceAsync(string deviceId)
    {
        var command = this.Connection.Value.CreateCommand();
        command.CommandText = DeviceDeletionCommand;

        command.Parameters.AddWithValue($"${nameof(Device.Id)}", deviceId);

        await command.ExecuteNonQueryAsync();
    }

    private static Device ReadDeviceObject(SqliteDataReader reader)
        => new Device
        {
            Id = reader.GetString(0),
            Type = Enum.TryParse<DeviceType>(reader.GetString(1), out var typeValue)
                ? typeValue
                : DeviceType.Unknown,
            Name = reader.GetString(2),
            HostOrIPAddress = reader.GetString(3),
        };
}
