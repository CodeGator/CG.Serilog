using CG;
using CG.Validations;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using System;
using System.Diagnostics;

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

            // Add our standard serilog configuration.
            hostBuilder.UseSerilog((hostingContext, loggerConfiguration) => {
                 loggerConfiguration.ReadFrom.Configuration(
                     hostingContext.Configuration
                     )
                     .Enrich.WithExceptionDetails()
                     .Enrich.FromLogContext()
                     .Enrich.WithProperty("ApplicationName", AppDomain.CurrentDomain.FriendlyNameEx(true))
                     .Enrich.WithProperty("Environment", hostingContext.HostingEnvironment);
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

            // Add our standard serilog configuration.
            hostBuilder.UseSerilog((hostingContext, loggerConfiguration) => {
                loggerConfiguration.ReadFrom.Configuration(
                    hostingContext.Configuration.GetSection(sectionName)
                    )
                    .Enrich.WithExceptionDetails()
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("ApplicationName", AppDomain.CurrentDomain.FriendlyNameEx(true))
                    .Enrich.WithProperty("Environment", hostingContext.HostingEnvironment);
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
