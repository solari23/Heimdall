// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Security.Claims;
using Heimdall.Models;
using Microsoft.AspNetCore.Authentication;

namespace Heimdall.Server.Security;

public class HeimdallRolesClaimsTransformation : IClaimsTransformation
{
    private const string AzureADObjectIdClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";

    public HeimdallRolesClaimsTransformation(IUserIdentityService userService)
    {
        this.UserService = userService;
    }

    private IUserIdentityService UserService { get; }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (!principal.HasClaim(c => c.Type == HeimdallRole.ClaimType))
        {
            var role = HeimdallRole.NoRole;

            var oidClaim = principal.Claims
                .Where(c => c.Type == AzureADObjectIdClaimType)
                .FirstOrDefault();

            if (oidClaim is not null)
            {
                role = this.UserService.GetRoleForUserObjectId(oidClaim.Value);
            }

            var newIdentity = new ClaimsIdentity();
            newIdentity.AddClaim(new Claim(
                type: HeimdallRole.ClaimType,
                value: role,
                valueType: ClaimValueTypes.String,
                issuer: HeimdallRole.HeimdallClaimIssuer));
            principal.AddIdentity(newIdentity);
        }

        return Task.FromResult(principal);
    }
}
