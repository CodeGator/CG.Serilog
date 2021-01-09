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
        /// This method adds a standard serilog based logging service to the
        /// specified host builder.
        /// </summary>
        /// <param name="hostBuilder">The host builder to use for the operation.</param>
        /// <returns>The value of the <paramref name="hostBuilder"/> parameter, 
        /// for chaining calls together.</returns>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// one or more of the required parameters is missing or invalid.</exception>
        public static IHostBuilder AddStandardSerilog(
            this IHostBuilder hostBuilder
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(hostBuilder, nameof(hostBuilder));

            // Setup Serilog.
            hostBuilder.UseSerilog((hostingContext, loggerConfiguration) => {

                // Get the application's friendly name.
                var friendlyName = AppDomain.CurrentDomain.FriendlyNameEx(true);

                // Get the configuration section from the standard location.
                var section = hostingContext.Configuration.GetSection(
                    "Services:Logging"  // <-- trailing ':Serilog' is added by serilog libs
                    );

                // How should we setup Serilog?
                if (section.GetChildren().Any())
                {
                    // If we get here then we can setup Serilog logging services using 
                    //   whatever happens to be in the configuration section.

                    // Setup serilog from the configuration.
                    loggerConfiguration.ReadFrom.Configuration(
                        section
                        );
                }
                else
                {
                    // If we get here then nothing was configured in the standard
                    //   location for a Serilog logging service. So, we'll make some
                    //   assumptions here and try to come up with a decent "default"
                    //   configuration.

                    // Setup serilog with defaults.
                    loggerConfiguration
                        .Enrich.WithExceptionDetails()
                        .Enrich.FromLogContext()
                        .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                        .WriteTo.File(
                            formatter: new RenderedCompactJsonFormatter(),
                            path: $"{friendlyName}-.log",
                            rollingInterval: RollingInterval.Day,
                            rollOnFileSizeLimit: true
                            );
                }

                // Either way, we want to add these enrichers.
                loggerConfiguration.Enrich.WithProperty("ApplicationName", friendlyName);
                loggerConfiguration.Enrich.WithProperty("MachineName", Environment.MachineName);
                loggerConfiguration.Enrich.WithProperty("Environment", hostingContext.HostingEnvironment);
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
