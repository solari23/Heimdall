// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Models;
using Microsoft.Extensions.Options;

namespace Heimdall.Server.Security;

public interface IUserIdentityService
{
    HeimdallRole GetRoleForUserObjectId(string userOid);
}

public class UserIdentityService : IUserIdentityService
{
    public UserIdentityService(IOptions<UserIdentityServiceOptions> options)
    {
        this.Options = options.Value;
    }

    private UserIdentityServiceOptions Options { get; }

    public HeimdallRole GetRoleForUserObjectId(string userOid)
        => this.Options.RoleAssignments.TryGetValue(userOid, out var role) ? role : HeimdallRole.NoRole;
}
