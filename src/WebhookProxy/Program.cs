// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using Heimdall.CommonServices.Storage;

namespace Heimdall.WebhookProxy;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration
            .AddJsonFile(
                "heimdall-webhookproxy.appsettings.json",
                optional: false,
                reloadOnChange: true)
            .AddJsonFile(
                $"heimdall-webhookproxy.appsettings.{builder.Environment.EnvironmentName}.json",
                optional: true,
                reloadOnChange: true);

        // Add services to the container.
        builder.Services.AddControllers();

        builder.Services.Configure<SqliteStorageAccessOptions>(
            builder.Configuration.GetSection(nameof(SqliteStorageAccess)));
        builder.Services.AddSingleton<IStorageAccess, SqliteStorageAccess>();

        var app = builder.Build();
        app.MapControllers();
        app.Run();
    }
}
