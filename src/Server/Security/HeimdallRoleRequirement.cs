// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models;
using Microsoft.AspNetCore.Authorization;

namespace Heimdall.Server.Security;

/// <summary>
/// Encodes a minimum <see cref="HeimdallRole"/> for granting authorization.
/// </summary>
public class HeimdallRoleRequirement : IAuthorizationRequirement
{
    public HeimdallRoleRequirement(HeimdallRole role)
    {
        if (role == HeimdallRole.NoRole)
        {
            throw new ArgumentException($"'{role}' is not a valid Heimdall authorization role.");
        }

        this.Role = role;
    }

    public HeimdallRole Role { get; }
}
