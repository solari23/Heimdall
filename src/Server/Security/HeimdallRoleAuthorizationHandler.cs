// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models;
using Microsoft.AspNetCore.Authorization;

namespace Heimdall.Server.Security;

/// <summary>
/// Implements the logic of checking Heimdall roles based on
/// requirements from <see cref="HeimdallRoleRequirement"/>.
/// </summary>
public class HeimdallRoleAuthorizationHandler : AuthorizationHandler<HeimdallRoleRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        HeimdallRoleRequirement requirement)
    {
        var roleClaim = context.User.FindFirst(HeimdallRole.ClaimType);

        if (roleClaim is not null)
        {
            var role = (HeimdallRole)roleClaim.Value;

            if (role.Satisfies(requirement.Role))
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}
