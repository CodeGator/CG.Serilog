using CG.Validations;
using Microsoft.AspNetCore.Builder;
using Serilog;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// This class contains extension methods related to the <see cref="IApplicationBuilder"/>
    /// types.
    /// </summary>
    public static partial class ApplicationBuilderExtensions
    {
        // *******************************************************************
        // Public methods.
        // *******************************************************************

        #region Public methods

        /// <summary>
        /// This method adds standard middleware for logging requests through
        /// the serilog library.
        /// </summary>
        /// <param name="applicationBuilder">The application builder to use 
        /// for the operation.</param>
        /// <returns>The value of the <paramref name="applicationBuilder"/>
        /// parameter, for chaining calls together.</returns>
        public static IApplicationBuilder UseSerilogRequestLogging(
            this IApplicationBuilder applicationBuilder
            )
        {
            // Validate the parameters before attempting to use them.
            Guard.Instance().ThrowIfNull(applicationBuilder, nameof(applicationBuilder));

            // Use serilog.
            SerilogApplicationBuilderExtensions.UseSerilogRequestLogging(applicationBuilder);

            // Return the builder.
            return applicationBuilder;
        }

        #endregion
    }
}
