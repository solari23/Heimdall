﻿// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

using Heimdall.CommonServices.Security;
using Heimdall.CommonServices.Storage;
using Heimdall.Integrations;
using Heimdall.Integrations.Shelly;
using Heimdall.Integrations.Tasmota;
using Heimdall.Server.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;

namespace Heimdall.Server;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration
            .AddJsonFile(
                "heimdall.appsettings.json",
                optional: false,
                reloadOnChange: true)
            .AddJsonFile(
                $"heimdall.appsettings.{builder.Environment.EnvironmentName}.json",
                optional: true,
                reloadOnChange: true);

        builder.WebHost.UseKestrel(kestrelOptions =>
        {
            kestrelOptions.Limits.MaxConcurrentConnections = 10_000;
            kestrelOptions.Limits.MaxRequestBodySize = 50 * 1024;

            var sslCert = GetServerSslCertificate();
            if (sslCert != null)
            {
                kestrelOptions.ConfigureHttpsDefaults(httpsOptions =>
                {
                    httpsOptions.ServerCertificate = sslCert;
                });
            }
        });

        // Add services to the container.
        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddScheme<AuthenticationSchemeOptions, HeimdallSecretKeyAuthenticationHandler>(
                HeimdallSecretKey.SchemeName,
                opts => { })
            .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
        builder.Services.AddAuthorization(options =>
        {
            // Set up the default policy that will apply to any controller/action
            // tagged with the plain [Authorize] attribute.
            options.DefaultPolicy = new AuthorizationPolicyBuilder()

                // The user must be authenticated.
                .RequireAuthenticatedUser()

                // Allows either HeimdallSecretKey or JwtBearer (AAD) authentication.
                .AddAuthenticationSchemes(
                    HeimdallSecretKey.SchemeName,
                    JwtBearerDefaults.AuthenticationScheme)
                .Build();
        });
        builder.Services.AddHeimdallRoleAuthorization();
        builder.Services.AddSingleton<HeimdallSecretKey>();

        builder.Services.Configure<UserIdentityServiceOptions>(
            builder.Configuration.GetSection(nameof(UserIdentityService)));
        builder.Services.AddSingleton<IUserIdentityService, UserIdentityService>();
        builder.Services.AddTransient<IClaimsTransformation, HeimdallRolesClaimsTransformation>();

        builder.Services.Configure<SqliteStorageAccessOptions>(
            SqliteStorageAccessOptions.Instances.Main,
            builder.Configuration.GetSection("SqliteStorageAccess:Main"));
        builder.Services.AddSingleton<IMainStorageAccess, MainStorageAccess>();

        builder.Services.Configure<SqliteStorageAccessOptions>(
            SqliteStorageAccessOptions.Instances.Event,
            builder.Configuration.GetSection("SqliteStorageAccess:Event"));
        builder.Services.AddSingleton<IEventStorageAccess, EventStorageAccess>();

        builder.Services.AddSingleton<ShellyClient>();
        builder.Services.AddSingleton<TasmotaClient>();
        builder.Services.AddTransient<IDeviceControllerFactory, DeviceControllerFactory>();

        builder.Services.AddControllers()
            .AddJsonOptions(opts =>
            {
                opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }

        app.UseHsts();
        app.UseHttpsRedirection();

        app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapFallbackToFile("index.html");

        app.Run();
    }

    /// <summary>
    /// Checks environment variable SSL_CERTIFICATE_DIRECTORY to see if an SSL cert
    /// is configured. If so, loads the certificate from files:
    ///   - Cert file called 'fullchain.pem'
    ///   - Private key from 'privkey.pem'
    ///
    /// Returns null if the setting isn't set (server should use default cert).
    /// </summary>
    private static X509Certificate2 GetServerSslCertificate()
    {
        const string CertDirEnvVar = "SSL_CERTIFICATE_DIRECTORY";
        var certDir = Environment.GetEnvironmentVariable(CertDirEnvVar);

        if (string.IsNullOrWhiteSpace(certDir))
        {
            Console.WriteLine(
                $"No SSL cert directory configured in environment var {CertDirEnvVar}. Server will load default.");
            return null;
        }

        Console.WriteLine($"Loading certificate from directory '{certDir}'");

        return X509Certificate2.CreateFromPemFile(
            Path.Join(certDir, "fullchain.pem"),
            Path.Join(certDir, "privkey.pem"));
    }
}
