// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Text.Json;

namespace Heimdall.Integrations.Tasmota;

public class TasmotaClient : IDisposable
{
    private const string GetPowerStateCommand = "POWER";

    private const string TogglePowerStateCommand = "POWER%20TOGGLE";

    private const string TurnPowerOnCommand = "POWER%20ON";

    private const string TurnPowerOffCommand = "POWER%20OFF";

    public TasmotaClient(HttpClient client = null)
    {
        this.Client = client ?? new HttpClient();
    }

    private HttpClient Client { get; }

    public async Task<PowerStateResponse> GetSwitchStatusAsync(Uri switchBaseUri, CancellationToken cancellationToken = default)
        => await this.SendCommandAsync<PowerStateResponse>(
            switchBaseUri,
            GetPowerStateCommand,
            cancellationToken);

    public async Task<PowerStateResponse> ToggleSwitchAsync(Uri switchBaseUri, CancellationToken cancellationToken = default)
        => await this.SendCommandAsync<PowerStateResponse>(
            switchBaseUri,
            TogglePowerStateCommand,
            cancellationToken);

    public async Task<PowerStateResponse> SetSwitchAsync(
        Uri switchBaseUri,
        bool setOn,
        CancellationToken cancellationToken = default)
        => await this.SendCommandAsync<PowerStateResponse>(
            switchBaseUri,
            setOn ? TurnPowerOnCommand : TurnPowerOffCommand,
            cancellationToken);

    private async Task<TResponse> SendCommandAsync<TResponse>(
        Uri baseUri,
        string command,
        CancellationToken cancellationToken)
    {
        var requestUriBuilder = new UriBuilder(baseUri);
        requestUriBuilder.Path = "cm";
        requestUriBuilder.Query = $"cmnd={command}";

        using var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUriBuilder.Uri);

        using var httpResponse = await this.Client.SendAsync(httpRequest, cancellationToken);
        httpResponse.EnsureSuccessStatusCode();

        var responseJson = await httpResponse.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(responseJson);
    }

    #region IDisposable Support

    private bool isDisposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.isDisposed)
        {
            if (disposing)
            {
                this.Client?.Dispose();
            }

            this.isDisposed = true;
        }
    }

    void IDisposable.Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion
}
