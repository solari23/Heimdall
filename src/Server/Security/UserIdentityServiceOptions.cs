// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

namespace Heimdall.Server.Security;

public class UserIdentityServiceOptions
{
    public Dictionary<string, string> RoleAssignments { get; set; } = new Dictionary<string, string>();
}
