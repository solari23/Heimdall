// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace Heimdall.CommonServices.Security;

public class HeimdallSecretKey
{
    public const string SchemeName = nameof(HeimdallSecretKey);

    public const string SecretKeyFilePathConfigKey = "HeimdallSecretKeyFile";

    public HeimdallSecretKey(IConfiguration config)
    {
        var keyFilePath = config.GetValue<string>(SecretKeyFilePathConfigKey);

        if (File.Exists(keyFilePath))
        {
            this.LoadedKey = File.ReadAllText(keyFilePath);
        }
    }

    private string LoadedKey { get; }

    public bool Authenticate(string inputKey)
        => StringSecureEquals(this.LoadedKey, inputKey);

    public AuthenticationHeaderValue ToAuthenticationHeader()
        => new AuthenticationHeaderValue(SchemeName, this.LoadedKey);

    private static bool StringSecureEquals(string left, string right)
    {
        if (string.IsNullOrWhiteSpace(left)
            || string.IsNullOrWhiteSpace(right))
        {
            return false;
        }

        // Compare all characters available with no short-circuiting.
        bool doStringsMatch = left.Length == right.Length;
        for (int i = 0; i < left.Length && i < right.Length; i++)
        {
            doStringsMatch &= left[i] == right[i];
        }

        return doStringsMatch;
    }
}
