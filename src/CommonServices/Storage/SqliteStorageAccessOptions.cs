﻿// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

namespace Heimdall.CommonServices.Storage;

public class SqliteStorageAccessOptions
{
    public static class Instances
    {
        public const string Main = nameof(Main);
        public const string Event = nameof(Event);
    }

    public string DatabaseFilePath { get; set; }

    public bool ReadOnly { get; set; } = false;
}
