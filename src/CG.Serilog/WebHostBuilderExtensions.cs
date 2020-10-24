﻿using CG.Validations;
using Serilog;

namespace Microsoft.AspNetCore.Hosting
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IWebHostBuilder"/>
    /// types.
    /// </summary>
    public static partial class WebHostBuilderExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method adds standard middleware for serilog logging.
        /// </summary>
        /// <param name="webHostBuilder">The application builder to use 
        /// for the operation.</param>
        /// <returns>The value of the <paramref name="webHostBuilder"/>
        /// parameter, for chaining calls together.</returns>
        public static IWebHostBuilder UseSerilog(
            this IWebHostBuilder webHostBuilder
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(webHostBuilder, nameof(webHostBuilder));

            // Use serilog.
            webHostBuilder.UseSerilog();

            // Return the builder.
            return webHostBuilder;
        }

        #endregion
    }
}
