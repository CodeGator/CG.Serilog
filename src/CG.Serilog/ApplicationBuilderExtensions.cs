using CG.Validations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IApplicationBuilder"/>
    /// types, for registering types related to serilog
    /// </summary>
    public static partial class ApplicationBuilderExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method wires up any logic required for the standard Serilog logging
        /// environment.
        /// </summary>
        /// <param name="applicationBuilder">The application builder to use 
        /// for the operation.</param>
        /// <param name="hostEnvironment">The host environment to use for the 
        /// operation.</param>
        /// <returns>The value of the <paramref name="applicationBuilder"/>
        /// parameter, for chaining calls together.</returns>
        /// <exception cref="ArgumentException">This exception is thrown whenever
        /// one or more of the required parameters is missing or invalid.</exception>
        public static IApplicationBuilder UseSerilogLoggerStrategies(
            this IApplicationBuilder applicationBuilder,
            IHostEnvironment hostEnvironment
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(applicationBuilder, nameof(applicationBuilder))
                .ThrowIfNull(hostEnvironment, nameof(hostEnvironment));

            //NOTE : nothing to do, yet.

            // Return the builder.
            return applicationBuilder;
        }

        #endregion
    }
}
