// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Heimdall.Models;

namespace Heimdall.Web;

public static class Helpers
{
    public static readonly JsonSerializerOptions DefaultJsonOptions = CreateDefaultJsonOptions();

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

    public static bool HasDeviceAdminAccess(this ClaimsPrincipal principal)
    {
        var heimdallRoleClaim = principal.FindFirst(HeimdallRole.ClaimType);

        if (heimdallRoleClaim is not null)
        {
            HeimdallRole role = heimdallRoleClaim.Value;
            return role.Satisfies(HeimdallRole.UberAdminRole);
        }

        return false;
    }

    private static JsonSerializerOptions CreateDefaultJsonOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }
}
