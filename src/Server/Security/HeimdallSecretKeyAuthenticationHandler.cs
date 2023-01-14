// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Security.Claims;
using System.Text.Encodings.Web;
using Heimdall.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Heimdall.Server.Security;

public class HeimdallSecretKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public static string SchemeName = "HeimdallSecretKey";

    public const string SecretKeyAuthenticationType = "HeimdallSecretKey";

    public HeimdallSecretKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IConfiguration config)
        : base(options, logger, encoder, clock)
    {
        this.Config = config;
    }

    private IConfiguration Config { get; }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        await Task.Yield();

        if (!this.Request.Headers.TryGetValue("Authorization", out var headerValues))
        {
            return AuthenticateResult.NoResult();
        }

        var authHeader = headerValues[0];
        if (!authHeader.StartsWith(SchemeName))
        {
            return AuthenticateResult.NoResult();
        }

        var key = authHeader.Substring(SchemeName.Length).Trim();
        if (string.IsNullOrWhiteSpace(key))
        {
            return AuthenticateResult.Fail("Missing key");
        }

        // TODO: Secret keys are only allowed on private IPs (on the local network).

        // TODO: Implement key.
        if (key != "woohoo")
        {
            return AuthenticateResult.Fail("Invalid key");
        }

        // TODO: Role based on key.
        return AuthenticateResult.Success(this.CreateAuthenticatedTicket(HeimdallRole.NoRole));
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
        return new AuthenticationTicket(principal, SchemeName);
    }
}
