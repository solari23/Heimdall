// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Security.Claims;

using Heimdall.Models;
using Heimdall.Web.Shared;
using Microsoft.JSInterop;

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

    public static ValueTask<bool> ConfirmAsync(this IJSRuntime jsRuntime, string prompt)
        => jsRuntime.InvokeAsync<bool>("confirm", prompt);

    public static ValueTask CopyToClipboardAsync(this IJSRuntime jsRuntime, string text)
        => jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
}
