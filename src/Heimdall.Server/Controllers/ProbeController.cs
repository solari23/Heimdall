// Copyright (c) Alexandre Kerametlian.
// Licensed under the Apache License, Version 2.0.

using System.Reflection;

using Microsoft.AspNetCore.Mvc;

namespace Heimdall.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ProbeController : ControllerBase
{
    private ILogger<ProbeController> Logger { get; init; }

    public ProbeController(ILogger<ProbeController> logger)
    {
        this.Logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        await Task.Yield();
        return this.Ok(ProbeResponse.Default);
    }

    private class ProbeResponse
    {
        public static ProbeResponse Default { get; } = new ProbeResponse();

        public string ProductName { get; init; }

        public string Version { get; init; }

        private ProbeResponse()
        {
            var assembly = typeof(ProbeController).Assembly;

            this.ProductName =
                assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title
                ?? string.Empty;

            this.Version = 
                assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? string.Empty;
        }
    }
}
