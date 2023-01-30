// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.Web.Shared;
using TimeZoneConverter;

namespace Heimdall.Web;

public static class TimeUtil
{
    public static readonly TimeZoneInfo DefaultTimeZone
        = TZConvert.GetTimeZoneInfo("Pacific Standard Time");

    public static DateTimeOffset GetStartTimeForTimeRange(
        TimeRange range,
        DateTimeOffset? currentTimeUtc = null)
    {
        currentTimeUtc ??= DateTimeOffset.UtcNow;

        if (range == TimeRange.Unset)
        {
            return currentTimeUtc.Value;
        }
        else if (range == TimeRange.AllTime)
        {
            return DateTimeOffset.MinValue;
        }
        else
        {
            var offset = range switch
            {
                TimeRange.LastDay => TimeSpan.FromDays(1),
                TimeRange.Last7Days => TimeSpan.FromDays(7),
                TimeRange.Last14Days => TimeSpan.FromDays(7),
                TimeRange.Last30Days => TimeSpan.FromDays(30),
                _ => throw new NotImplementedException(),
            };

            return currentTimeUtc.Value - offset;
        }
    }

    public static DateTimeOffset ChangeTimezone(
        this DateTimeOffset originalTime,
        TimeZoneInfo timeZone = null)
    {
        timeZone ??= DefaultTimeZone;
        return originalTime.ToOffset(timeZone.GetUtcOffset(originalTime));
    }
}
