using CG;
using CG.Diagnostics;
using CG.Validations;
using Microsoft.Extensions.Hosting;
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
