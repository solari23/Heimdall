// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

namespace Heimdall.CommonServices.Storage;

public static class IdGenerator
{
    public static string CreateNewId()
        => ConvertToBase64Url(Guid.NewGuid().ToByteArray());

    public static string ConvertToBase64Url(byte[] bytes)
        => Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}
