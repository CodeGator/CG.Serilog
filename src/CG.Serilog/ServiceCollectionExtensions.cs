using CG.Validations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Diagnostics;
using System.Linq;

namespace CG.Serilog
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IServiceCollection"/>
    /// types, for registering types related to serilog.
    /// </summary>
    public static partial class ServiceCollectionExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method adds standard serilog based logging strategies to the
        /// logging service.
        /// </summary>
        /// <param name="serviceCollection">The service collection to use for 
        /// the operation.</param>
        /// <param name="serviceLifetime">The service lifetime to use for the operation.</param>
        /// <param name="configuration">The configuration to use for the operation.</param>
        /// <returns>The value of the <paramref name="serviceCollection"/>
        /// parameter, for chaining calls together.</returns>
        public static IServiceCollection AddSerilogLoggingStrategies(
            this IServiceCollection serviceCollection,
            IConfiguration configuration,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(serviceCollection, nameof(serviceCollection))
                .ThrowIfNull(configuration, nameof(configuration));

            // Get the application's friendly name.
            var friendlyName = AppDomain.CurrentDomain.FriendlyNameEx(true);

            // Create a logger configuration, for Serilog.
            var loggerConfiguration = new LoggerConfiguration();

            // Are we already pointed at the Serilog section?
            if (configuration.GetPath().EndsWith("Serilog"))
            {
                // If we get here then we have to back up a level because the
                //   Serilog method we'll use, to read the configuration section
                //   from, wants to move down, internally, into a 'Serilog' section,
                //   before it reads the configuration values. 

                // Get the parent section.
                configuration = configuration.GetParentSection();
            }

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
            // Register the logger configuration.
            serviceCollection.AddSingleton(loggerConfiguration);

            // Register a logger factory.
            serviceCollection.AddSingleton<ILoggerFactory>(services =>
            {
                // Get the logger configuration.
                var loggerConfiguration = services.GetRequiredService<LoggerConfiguration>();

                // Write our configuration out as providers.
                var providerCollection = new LoggerProviderCollection();
                loggerConfiguration.WriteTo.Providers(
                    providerCollection
                    );

                // Create the logger factory.
                var factory = new SerilogLoggerFactory(
                    null,
                    true,
                    providerCollection
                    );

                // Return the logger factory.
                return factory;
            });

            // Create the static Serilog logger instance.
            Log.Logger = loggerConfiguration.CreateLogger();

            // Register the logger instance.
            serviceCollection.AddSingleton(Log.Logger);

            // Return the service collection.
            return serviceCollection;
        }

        #endregion
    }
}
