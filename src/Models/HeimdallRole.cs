// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

namespace Heimdall.Models;

public class HeimdallRole
{
    public const string HeimdallClaimIssuer = "idp://heimdall";

    public const string ClaimType = "HeimdallRole";

    // Uber admins can modify the home configuration.
    public const string UberAdmin = "Heimdall-UberAdmin";
    public static readonly HeimdallRole UberAdminRole = new (UberAdmin, 100);

    // Secret key is used by other Heimdall services on the local node.
    // These services should have admin-leven access to home devices but no
    // ability to modify the home configuration.
    public const string SecretKeyUser = "Heimdall-SecretKeyUser";
    public static readonly HeimdallRole SecretKeyUserRole = new(SecretKeyUser, 75);

    // HomeAdmins can modify the state of devices (e.g. turn on lights, etc.)
    public const string HomeAdmin = "Heimdall-HomeAdmin";
    public static readonly HeimdallRole HomeAdminRole = new (HomeAdmin, 70);

    // HomeViewers can only view the current state of devices but not modify anything.
    public const string HomeViewer = "Heimdall-HomeViewer";
    public static readonly HeimdallRole HomeViewerRole = new (HomeViewer, 10);

    public static readonly HeimdallRole NoRole = new ("NoRole", 0);

    private static readonly IReadOnlyDictionary<string, HeimdallRole> RoleMap
        = new Dictionary<string, HeimdallRole>(StringComparer.OrdinalIgnoreCase)
        {
            { UberAdminRole.Name, UberAdminRole },
            { SecretKeyUserRole.Name, SecretKeyUserRole },
            { HomeAdminRole.Name, HomeAdminRole },
            { HomeViewerRole.Name, HomeViewerRole },
            { NoRole.Name, NoRole },
        };

    private HeimdallRole(string name, int level)
    {
        this.Name = name;
        this.Level = level;
    }

    public string Name { get; }

    public int Level { get; }

    public static implicit operator string(HeimdallRole role) => role?.Name;

    public static implicit operator HeimdallRole(string roleString)
        => RoleMap.TryGetValue(roleString, out var role) ? role : NoRole;

    public static HeimdallRole FromString(string roleString) => roleString;

    public override string ToString() => this;

    public bool Satisfies(HeimdallRole requiredRole)
    {
        ArgumentNullException.ThrowIfNull(requiredRole);
        return this.Level >= requiredRole.Level;
    }
}
