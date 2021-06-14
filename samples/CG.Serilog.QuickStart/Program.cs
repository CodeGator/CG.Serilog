using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace CG.Serilog.QuickStart
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>(); // < -- call our startup class ...
                })
                .Build();

            // Let's verfiy that we actually registered Serilog ...
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogTrace("trace through serilog.");
            logger.LogDebug("debug through serilog.");
            logger.LogInformation("information through serilog.");
            logger.LogWarning("warning through serilog.");
            logger.LogError("error through serilog.");
            logger.LogCritical("critical through serilog.");

            host.Start();
        }
    }
}
