// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Microsoft.AspNetCore.Authorization;

namespace Heimdall.Server.Security;

public static class HeimdallSecurityExtensions
{
    /// <summary>
    /// Adds Heimdall Role-based AuthZ handlers to the DI container.
    /// </summary>
    public static void AddHeimdallRoleAuthorization(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationPolicyProvider, HeimdallRolePolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, HeimdallRoleAuthorizationHandler>();
    }
}
