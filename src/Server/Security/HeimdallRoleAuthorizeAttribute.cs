// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models;
using Microsoft.AspNetCore.Authorization;

namespace Heimdall.Server.Security;

/// <summary>
/// Allows tagging controllers and actions with an authorization attribute
/// that specifies the minimum <see cref="HeimdallRole"/> that a user must
/// possess in order to run the operation.
/// </summary>
public class HeimdallRoleAuthorizeAttribute : AuthorizeAttribute
{
    public HeimdallRoleAuthorizeAttribute(string heimdallRole)
    {
        HeimdallRole role = heimdallRole;

        if (role == HeimdallRole.NoRole)
        {
            throw new ArgumentException($"'{heimdallRole}' is not a valid Heimdall authorization role.");
        }

        // In AspNetCore, policies drive AuthZ and they are defined explicitly by their name.
        // We implement a custom PolicyProvider (HeimdallRolePolicyProvider) whose job will be to parse
        // this policy name and convert it into an AuthorizationPolicy object which can be evaluated.
        this.Policy = HeimdallRolePolicyProvider.PolicyPrefix + role;
    }
}
