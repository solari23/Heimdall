// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Text.Json.Serialization;
using Heimdall.CommonServices.Security;
using Heimdall.CommonServices.Storage;

namespace Heimdall.WebhookProxy;

public class Program
{
    public const string HeimdallApiHttpClientName = "Heimdall.ServerAPI";

    public const string HeimdallApiUriConfigKey = "HeimdallServerApiUri";

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
        builder.Services.AddControllers()
            .AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        builder.Services.AddSingleton<HeimdallSecretKey>();

        // Add the client to call Heimdall server APIs on the local node.
        // Configure the client to ignore SSL errors since we'll be calling on 'localhost'.
        builder.Services.AddHttpClient(
            HeimdallApiHttpClientName,
            (services, client) =>
            {
                var config = services.GetRequiredService<IConfiguration>();

                client.BaseAddress = config.GetValue<Uri>(HeimdallApiUriConfigKey);

                var secretKey = services.GetService<HeimdallSecretKey>();
                client.DefaultRequestHeaders.Authorization = secretKey.ToAuthenticationHeader();
            })
            .ConfigureHttpMessageHandlerBuilder(handlerBuilder =>
            {
                handlerBuilder.PrimaryHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback
                        = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                };
            });

        builder.Services.Configure<SqliteStorageAccessOptions>(
            SqliteStorageAccessOptions.Instances.Main,
            builder.Configuration.GetSection("SqliteStorageAccess:Main"));
        builder.Services.AddSingleton<IMainStorageAccess, SqliteStorageAccess>();

        builder.Services.AddSingleton<ActionProcessor>();

        var app = builder.Build();
        app.MapControllers();
        app.Run();
    }
}
