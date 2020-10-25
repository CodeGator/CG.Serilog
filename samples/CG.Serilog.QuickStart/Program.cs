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
                .ConfigureWebHost(hostBuilder =>
                {
                    hostBuilder.UseSerilog(); // This call is required.
                })
                .AddSerilog() // This call is required.
                .Build();

            // That's it! The host should now have a registered Serilog logger.

            host.RunDelegate(h =>
            {
                // Let's verfiy that we actually registered Serilog ...
                var logger = h.Services.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("information through serilog.");
                logger.LogWarning("warning through serilog.");
                logger.LogError("error through serilog.");

                // Yay, it worked.
                Console.WriteLine();
                Console.WriteLine("press any key to exit.");
                Console.ReadKey();
            });
        }
    }
}
