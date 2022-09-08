// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Heimdall.Server.Security;

/// <summary>
/// The Policy Provider's job is to parse out a policy name into an <see cref="AuthorizationPolicy"/>.
/// For example:
/// "HEIMDALL_ROLE_POLICY|Heimdall-HomeAdmin" -> Policy with a min requirement of Heimdall-HomeAdmin.
/// </summary>
public class HeimdallRolePolicyProvider : IAuthorizationPolicyProvider
{
    public const string PolicyPrefix = "HEIMDALL_ROLE_POLICY|";

    public HeimdallRolePolicyProvider(IOptions<AuthorizationOptions> options)
    {
        // AspNetCore only allows registering 1 IAuthorizationPolicyProvider.
        // Our custom provider might not be able to handle all cases. We
        // create this instance of the default provider to use as a fallback.
        this.DefaultProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    private IAuthorizationPolicyProvider DefaultProvider { get; }

    /// <inheritdoc />
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => this.DefaultProvider.GetDefaultPolicyAsync();

    /// <inheritdoc />
    public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => this.DefaultProvider.GetFallbackPolicyAsync();

    /// <inheritdoc />
    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(PolicyPrefix))
        {
            var role = (HeimdallRole)policyName.Substring(PolicyPrefix.Length);

            if (role == HeimdallRole.NoRole)
            {
                throw new ArgumentException($"Policy name '{policyName}' refers to an invalid Heimdall role; cannot construct AuthZ policy.");
            }

            var policy = new AuthorizationPolicyBuilder();
            policy.AddRequirements(new HeimdallRoleRequirement(role));
            return Task.FromResult(policy.Build());
        }

        // If this isn't a policy that we recognize, let the default provider take a look.
        return this.DefaultProvider.GetPolicyAsync(policyName);
    }
}
