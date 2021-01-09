using CG.Validations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CG.Serilog
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IServiceCollection"/>
    /// types.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method adds a standard serilog based logging service to the
        /// specified service collection.
        /// </summary>
        /// <param name="serviceCollection">The service collection to use for 
        /// the operation.</param>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <returns>The value of the <paramref name="serviceCollection"/>
        /// parameter, for chaining calls together.</returns>
        public static IServiceCollection AddStandardSerilog(
            this IServiceCollection serviceCollection,
            IConfiguration configuration
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(configuration, nameof(configuration));

            // Get the application's friendly name.
            var friendlyName = AppDomain.CurrentDomain.FriendlyNameEx(true);

            // Create a logger configuration, for Serilog.
            var loggerConfiguration = new LoggerConfiguration();

            // How should we setup Serilog?
            if (configuration.GetChildren().Any())
            {
                // If we get here then we can setup Serilog logging services using 
                //   whatever happens to be in the configuration section.

                // Setup serilog from the configuration.
                loggerConfiguration.ReadFrom.Configuration(
                    configuration
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
#if DEBUG
            loggerConfiguration.Enrich.WithProperty("DebuggerAttached", Debugger.IsAttached);
#endif

            // Set the configured logger, for Serilog.
            Log.Logger = loggerConfiguration.CreateLogger();

            // Return the service collection.
            return serviceCollection;
        }

        #endregion
    }
}
