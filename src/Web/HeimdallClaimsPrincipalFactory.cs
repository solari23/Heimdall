// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using Heimdall.Models;
using Heimdall.Models.Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

namespace Heimdall.Web;

/// <summary>
/// Adds Heimdall-specific claims to the AAD-based identity after it has authenticated.
/// </summary>
public class HeimdallClaimsPrincipalFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
{
    public HeimdallClaimsPrincipalFactory(
        IAccessTokenProviderAccessor accessor,
        NavigationManager navigation)
        : base(accessor)
    {
        this.HttpClient = new HttpClient();
        this.HttpClient.BaseAddress = new Uri(navigation.BaseUri);
    }

    private HttpClient HttpClient { get; }

    public override async ValueTask<ClaimsPrincipal> CreateUserAsync(
        RemoteUserAccount account,
        RemoteAuthenticationUserOptions options)
    {
        var user = await base.CreateUserAsync(account, options);

        if (user.Identity.IsAuthenticated)
        {
            string heimdallRole = HeimdallRole.NoRole;

            try
            {
                // Get an Access Token to call the Heimdall identity info API.
                var tokenResult = await this.TokenProvider.RequestAccessToken();
                if (tokenResult.TryGetToken(out var tokenObject))
                {
                    this.HttpClient.DefaultRequestHeaders.Authorization
                        = new AuthenticationHeaderValue("Bearer", tokenObject.Value);

                    var idInfo = await this.HttpClient.GetFromJsonAsync<IdentityInfo>("api/Identity");
                    heimdallRole = idInfo.HeimdallRole;
                }
            }
            catch (Exception)
            {
                // Something failed. Assume no role.
                heimdallRole = HeimdallRole.NoRole;
            }

            var userIdentity = (ClaimsIdentity)user.Identity;
            userIdentity.AddClaim(new Claim(
                type: HeimdallRole.ClaimType,
                value: heimdallRole));
        }

        return user;
    }
}
