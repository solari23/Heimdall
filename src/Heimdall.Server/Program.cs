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

        builder.WebHost.UseKestrel(o =>
        {
            o.Limits.MaxConcurrentConnections = 100;
            o.Limits.MaxRequestBodySize = 50 * 1024;
        });

        // Add services to the container.
        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

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
}
