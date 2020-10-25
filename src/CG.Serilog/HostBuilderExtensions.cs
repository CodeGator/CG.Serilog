using CG;
using CG.Validations;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IHostBuilder"/>
    /// types.
    /// </summary>
    public static partial class HostBuilderExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method adds a standard serilog configuration to the specified
        /// host builder.
        /// </summary>
        /// <param name="hostBuilder">The host builder to use for the operation.</param>
        /// <returns>The value of the <paramref name="hostBuilder"/> parameter, 
        /// for chaining calls together.</returns>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// one or more of the required parameters is missing or invalid.</exception>
        /// <remarks>
        /// <para>
        /// For our purposes, a 'standard' Serilog configuration is: 
        /// <list type="bullet">
        /// <item><description>Enriched with exception details.</description></item>
        /// <item><description>Enriched from the log context.</description></item>
        /// <item><description>Encriched with the application name.</description></item>
        /// <item><description>Encriched with the machine name.</description></item>
        /// <item><description>Encriched with whether there is a debugger attached.</description></item>
        /// <item><description>Enriched with the Environment name.</description></item>
        /// <item><description>Console logger with <c>AnsiConsoleTheme.Code</c> theme.</description></item>
        /// <item><description>File logger with compact JSON formatter, rolling daily.</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// If you specify a 'Serilog' section in your configuration, then you can, of course,
        /// choose whatever you like for a 'standard' configuration.
        /// </para>
        /// </remarks>
        /// <example>
        /// This example demostrates a typical use of the <see cref="AddSerilog(IHostBuilder)"/>
        /// method:
        /// <code>
        /// static void Main(string[] args)
        /// {
        ///     var host = Host.CreateDefaultBuilder()
        ///                    .ConfigureWebHost(hostBuilder =>
        ///                    {
        ///                        hostBuilder.UseSerilog(); // This call is also required.
        ///                    })
        ///                    .AddSerilog()
        ///                    .Build();
        /// }
        /// </code>
        /// </example>
        public static IHostBuilder AddSerilog(
            this IHostBuilder hostBuilder
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(hostBuilder, nameof(hostBuilder));

            // Setup Serilog.
            hostBuilder.UseSerilog((hostingContext, loggerConfiguration) => {

                // Get the application's friendly name.
                var friendlyName = AppDomain.CurrentDomain.FriendlyNameEx(true);

                // How should we setup Serilog?
                if (hostingContext.Configuration.GetSection("Serilog").GetChildren().Any())
                {
                    // If we get here, there is a populated 'Serilog' section in 
                    //   the configuration to read from.

                    // Setup serilog from the configuration.
                    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration)
                        .Enrich.WithExceptionDetails()
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("ApplicationName", friendlyName)
                        .Enrich.WithProperty("MachineName", Environment.MachineName)
                        .Enrich.WithProperty("Environment", hostingContext.HostingEnvironment);
                }
                else
                {
                    // If we get here, there is no 'Serilog' section in the 
                    //   configuration to read from.

                    // Setup serilog with defaults.
                    loggerConfiguration
                        .Enrich.WithExceptionDetails()
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("ApplicationName", friendlyName)
                        .Enrich.WithProperty("MachineName", Environment.MachineName)
                        .Enrich.WithProperty("Environment", hostingContext.HostingEnvironment)
                        .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                        .WriteTo.File(
                            formatter: new RenderedCompactJsonFormatter(),
                            path: $"{friendlyName}-.log",
                            rollingInterval: RollingInterval.Day,
                            rollOnFileSizeLimit: true
                            );
                }
#if DEBUG
                loggerConfiguration.Enrich.WithProperty("DebuggerAttached", Debugger.IsAttached);
#endif
             });

            // Return the builder.
            return hostBuilder;
        }

        // *******************************************************************

        /// <summary>
        /// This method adds a standard serilog configuration to the specified
        /// host builder.
        /// </summary>
        /// <param name="hostBuilder">The host builder to use for the operation.</param>
        /// <param name="sectionName">The configuration section to use for the 
        /// operation.</param>
        /// <returns>The value of the <paramref name="hostBuilder"/> parameter, 
        /// for chaining calls together.</returns>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// one or more of the required parameters is missing or invalid.</exception>
        /// <remarks>
        /// <para>
        /// If you specify a 'Serilog' section in your configuration, then of course,
        /// you can choose whatever you like for a 'standard' configuration.
        /// </para>
        /// </remarks>
        /// <example>
        /// This example demostrates a typical use of the <see cref="AddSerilog(IHostBuilder)"/>
        /// method:
        /// <code>
        /// static void Main(string[] args)
        /// {
        ///     var host = Host.CreateDefaultBuilder()
        ///                    .ConfigureWebHost(hostBuilder =>
        ///                    {
        ///                        hostBuilder.UseSerilog(); // This call is also required.
        ///                    })
        ///                    .AddSerilog("MySerilogSection")
        ///                    .Build();
        /// }
        /// </code>
        /// </example>
        public static IHostBuilder AddSerilog(
            this IHostBuilder hostBuilder,
            string sectionName
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(hostBuilder, nameof(hostBuilder))
                .ThrowIfNullOrEmpty(sectionName, nameof(sectionName));

            // Minimal Serilog configuration.
            hostBuilder.UseSerilog((hostingContext, loggerConfiguration) => {
                loggerConfiguration.ReadFrom.Configuration(
                    hostingContext.Configuration.GetSection(sectionName)
                    )
                    .Enrich.WithExceptionDetails()
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("ApplicationName", AppDomain.CurrentDomain.FriendlyNameEx(true))
                    .Enrich.WithProperty("Environment", hostingContext.HostingEnvironment)
                    .WriteTo.Console();
#if DEBUG
                loggerConfiguration.Enrich.WithProperty("DebuggerAttached", Debugger.IsAttached);
#endif
            });

            // Return the builder.
            return hostBuilder;
        }

        #endregion
    }
}
