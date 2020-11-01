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
            var host = Host.CreateDefaultBuilder()
                .ConfigureWebHost(webHostBuilder=> {
                    webHostBuilder.UseSerilog(); // If this was a web app.
                })
                .AddSerilog() // This call is required.
                .Build();

            // That's it! The host should now have a registered Serilog logger.

            host.RunDelegate(h =>
            {
                // Let's verfiy that we actually registered Serilog ...
                var logger = h.Services.GetRequiredService<ILogger<Program>>();
                logger.LogTrace("trace through serilog.");
                logger.LogDebug("debug through serilog.");
                logger.LogInformation("information through serilog.");
                logger.LogWarning("warning through serilog.");
                logger.LogError("error through serilog.");
                logger.LogCritical("critical through serilog.");

                Console.WriteLine();
                Console.WriteLine("press any key to exit.");
                Console.ReadKey();
            });
        }
    }
}
