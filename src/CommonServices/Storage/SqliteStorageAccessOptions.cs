// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

namespace Heimdall.CommonServices.Storage;

public class SqliteStorageAccessOptions
{
    public string DatabaseFilePath { get; set; }

    public bool ReadOnly { get; set; } = false;
}
