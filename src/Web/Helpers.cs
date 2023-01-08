// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Security.Claims;
using Heimdall.Models;

namespace Heimdall.Web;

public static class Helpers
{
    public static bool HasReadLevelAccess(this ClaimsPrincipal principal)
    {
        var heimdallRoleClaim = principal.FindFirst(HeimdallRole.ClaimType);

        if (heimdallRoleClaim is not null)
        {
            HeimdallRole role = heimdallRoleClaim.Value;
            return role.Satisfies(HeimdallRole.HomeViewerRole);
        }

        return false;
    }

    public static bool HasWriteLevelAccess(this ClaimsPrincipal principal)
    {
        var heimdallRoleClaim = principal.FindFirst(HeimdallRole.ClaimType);

        if (heimdallRoleClaim is not null)
        {
            HeimdallRole role = heimdallRoleClaim.Value;
            return role.Satisfies(HeimdallRole.HomeAdminRole);
        }

        return false;
    }
}
