// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

namespace Heimdall.Server;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureDIContainerServices(builder.Services);

        var app = builder.Build();
        ConfigureApplicationPipeline(app);

        app.Run();
    }

    public static void ConfigureDIContainerServices(IServiceCollection services)
    {
        services.AddControllers();
    }

    public static void ConfigureApplicationPipeline(WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
    }
}
