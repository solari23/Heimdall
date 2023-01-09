// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Collections.Concurrent;
using System.Text;

namespace Heimdall.Web;

public static class EnumUtil<TEnum> where TEnum : Enum
{
    private static readonly ConcurrentDictionary<TEnum, string> PrettyStringCache
        = new ConcurrentDictionary<TEnum, string>();

    public static string ToPrettyString(TEnum value)
        => PrettyStringCache.GetOrAdd(value, v => AddSpacesToCamelCase(v.ToString()));

    private static string AddSpacesToCamelCase(string s)
    {
        var builder = new StringBuilder(s.Length);

        for (int i = 0; i < s.Length; i++)
        {
            if (char.IsUpper(s[i]))
            {
                builder.Append(' ');
            }
            builder.Append(s[i]);
        }

        return builder.ToString();
    }
}
