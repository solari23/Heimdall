// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;

using Heimdall.CommonServices.Security;
using Heimdall.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Heimdall.Server.Security;

public class HeimdallSecretKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SecretKeyAuthenticationType = "HeimdallSecretKey";

    public HeimdallSecretKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IConfiguration config,
        HeimdallSecretKey secretKey)
        : base(options, logger, encoder, clock)
    {
        this.Config = config;
        this.SecretKey = secretKey;
    }

    private IConfiguration Config { get; }

    private HeimdallSecretKey SecretKey { get; }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!this.Request.Headers.TryGetValue("Authorization", out var headerValues))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var authHeader = headerValues[0];
        if (!authHeader.StartsWith(HeimdallSecretKey.SchemeName))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var inputKey = authHeader.Substring(HeimdallSecretKey.SchemeName.Length).Trim();
        if (string.IsNullOrWhiteSpace(inputKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing key"));
        }

        // Secret keys are only allowed on localhost IPs.
        var clientIP = this.Request.HttpContext.Connection.RemoteIpAddress;
        if (clientIP is null || !IPAddress.IsLoopback(clientIP))
        {
            return Task.FromResult(AuthenticateResult.Fail("Client secret not allowed from outside of localhost"));
        }

        if (!this.SecretKey.Authenticate(inputKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid key"));
        }

        var ticket = this.CreateAuthenticatedTicket(HeimdallRole.SecretKeyUserRole);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private AuthenticationTicket CreateAuthenticatedTicket(HeimdallRole role)
    {
        var claims = new Claim[]
        {
            new Claim(HeimdallRole.ClaimType, role),

            // Controllers looking for AAD auth will be configured
            // to check for a specific scope of access. Add the 'scp'
            // claim with the expected values to satisfy AAD auth filters.
            new Claim(
                AadHelpers.ScopesClaimType,
                this.Config[AadHelpers.RequiredScopesConfigKey]),
        };

        var identity = new ClaimsIdentity(
            claims,
            authenticationType: SecretKeyAuthenticationType);
        var principal = new ClaimsPrincipal(identity);
        return new AuthenticationTicket(principal, HeimdallSecretKey.SchemeName);
    }
}
