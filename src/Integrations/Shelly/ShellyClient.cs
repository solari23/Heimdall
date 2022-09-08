// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Text.Json;
using Heimdall.Integrations.Shelly.Messages;

namespace Heimdall.Integrations.Shelly;

public class ShellyClient : IDisposable
{
    public ShellyClient(HttpClient client = null)
    {
        this.Client = client ?? new HttpClient();
    }

    private HttpClient Client { get; }

    public async Task<SwitchStatus> GetSwitchStatusAsync(Uri switchBaseUri, CancellationToken cancellationToken = default)
        => await this.SendRequestAsync(
            switchBaseUri,
            new SwitchGetStatusRequest(),
            cancellationToken);

    public async Task<SwitchPreviousState> ToggleSwitchAsync(Uri switchBaseUri, CancellationToken cancellationToken = default)
        => await this.SendRequestAsync(
            switchBaseUri,
            new SwitchToggleRequest(),
            cancellationToken);

    public async Task<SwitchPreviousState> SetSwitchAsync(
        Uri switchBaseUri,
        bool setOn,
        TimeSpan? toggleBackAfter = null,
        CancellationToken cancellationToken = default)
        => await this.SendRequestAsync(
            switchBaseUri,
            new SwitchSetRequest(setOn)
            {
                ToggleBackAfterSeconds = toggleBackAfter.HasValue
                    ? toggleBackAfter.Value.TotalSeconds
                    : null,
            },
            cancellationToken);

    private async Task<TResponse> SendRequestAsync<TResponse>(
        Uri baseUri,
        ShellyRequestMessage<TResponse> request,
        CancellationToken cancellationToken)
    {
        var requestUri = new Uri(
            baseUri,
            $"rpc/{request.MethodName}");

        var requestJson = JsonSerializer.Serialize(request, request.GetType());
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = new StringContent(requestJson),
        };

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
