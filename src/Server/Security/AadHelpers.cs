// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

namespace Heimdall.Server.Security;

public static class AadHelpers
{
    public const string ObjectIdClaimType = "http://schemas.microsoft.com/identity/claims/objectidentifier";

    public const string ScopesClaimType = "scp";

    public const string RequiredScopesConfigKey = "AzureAd:Scopes";
}
