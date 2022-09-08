using System.Security.Cryptography.X509Certificates;

using Heimdall.Server.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

namespace Heimdall.Server;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

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
            .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
        builder.Services.AddHeimdallRoleAuthorization();

        builder.Services.Configure<UserIdentityServiceOptions>(
            builder.Configuration.GetSection(nameof(UserIdentityService)));
        builder.Services.AddSingleton<IUserIdentityService, UserIdentityService>();
        builder.Services.AddTransient<IClaimsTransformation, HeimdallRolesClaimsTransformation>();

        builder.Services.AddControllers();

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
