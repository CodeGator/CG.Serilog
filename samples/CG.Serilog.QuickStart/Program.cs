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
                    hostBuilder.UseSerilog();
                })
                .AddSerilog()
                .Build();

            host.RunDelegate(h =>
            {
                var logger = h.Services.GetRequiredService<ILogger<Program>>();
                
                logger.LogInformation("information through serilog.");
                logger.LogWarning("warning through serilog.");
                logger.LogError("error through serilog.");

                Console.WriteLine();
                Console.WriteLine("press any key to exit.");
                Console.ReadKey();
            });
        }
    }
}
