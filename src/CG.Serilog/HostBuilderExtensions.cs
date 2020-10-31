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
        /// <remarks>
        /// <remarks>
        /// <para>
        /// The idea with this method, is to create a quick and easy way to setup 
        /// logging services for a hosted application by following a simple configuration 
        /// convention. That convention assumes a configuration section exists like this:
        /// <code language="json">
        /// {
        ///    "Services" : {
        ///       "Logging": {
        ///          "Strategy" : "Serilog",
        ///          "Assembly" : "",
        ///          "Serilog": {
        ///             "Using": [ "Serilog.Sinks.Console" ],
        ///             "MinimumLevel": "Information",
        ///             "WriteTo": [{
        ///                "Name": "Console",
        ///                "Args": { "theme": "Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme::Code, Serilog.Sinks.Console" }
        ///             }]
        ///          }
        ///       }
        ///    }
        /// }
        /// </code>
        /// </para>
        /// <para>
        /// Let's break it down. The <c>Services</c> section is where ALL services for the 
        /// application should be configured. If you're adding a CodeGator service to your
        /// application then you'll want to add that service to this section in order to 
        /// configure the service at startup.
        /// </para>
        /// <para>
        /// Under <c>Services</c>, the <c>Logging</c> section is where the CodeGator logging service 
        /// should be configured. This section contains at least two nodes: <c>Strategy</c> and 
        /// <c>Assembly</c>. The <c>Strategy</c> node tells the host what strategy to load
        /// for the logging service, and as such, is required. The <c>Assembly</c> section is 
        /// optional, and is only needed when the strategy is located in an external assembly 
        /// that should be dynamically loaded at startup.
        /// </para>
        /// <para>
        /// The <c>Serilog</c> section is an example of a strategy section. This will vary, of
        /// course, depending on which strategy is named in the <c>Strategy</c> node. In this
        /// case, we see a made up example for a Serilog based logging strategy. One thing to 
        /// note is, the actual content of the <c>Serilog</c> strategy is determined by the 
        /// Serilog library, not by this library. All we do here is route the appropriate configuration
        /// section to the Serilog library at startup.
        /// </para>
        /// </remarks>
        /// <example>
        /// This example demostrates a typical use of the <see cref="AddSerilog(IHostBuilder)"/>
        /// method:
        /// <code>
        /// static void Main(string[] args)
        /// {
        ///     var host = Host.CreateDefaultBuilder()
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

                // Get the configuration section from the standard location.
                var section = hostingContext.Configuration.GetSection(
                    "Services:Logging:Serilog"
                    );

                // How should we setup Serilog?
                if (section.GetChildren().Any())
                {
                    // If we get here then we can setup Serilog logging services using 
                    //   whatever happens to be in the configuration section.

                    // Setup serilog from the configuration.
                    loggerConfiguration.ReadFrom.Configuration(
                        hostingContext.Configuration
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
