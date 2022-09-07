// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

namespace Heimdall.Models;

public class HeimdallRole
{
    public const string HeimdallClaimIssuer = "idp://heimdall";

    public const string ClaimType = "HeimdallRole";

    public static readonly HeimdallRole UberAdmin = new ("Heimdall-UberAdmin", 100);

    public static readonly HeimdallRole HomeAdmin = new ("Heimdall-HomeAdmin", 70);

    public static readonly HeimdallRole HomeViewer = new ("Heimdall-HomeViewer", 10);

    public static readonly HeimdallRole NoRole = new ("NoRole", 0);

    private static readonly IReadOnlyDictionary<string, HeimdallRole> RoleMap
        = new Dictionary<string, HeimdallRole>(StringComparer.OrdinalIgnoreCase)
        {
            { UberAdmin.Name, UberAdmin },
            { HomeAdmin.Name, HomeAdmin },
            { HomeViewer.Name, HomeViewer },
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

    public bool Satisfies(HeimdallRole requiredRole)
    {
        ArgumentNullException.ThrowIfNull(requiredRole);
        return this.Level >= requiredRole.Level;
    }
}
