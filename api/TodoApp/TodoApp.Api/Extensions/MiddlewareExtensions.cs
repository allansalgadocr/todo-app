using TodoApp.Api.Middleware;
using TodoApp.Api.Options;

namespace TodoApp.Api.Extensions
{
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// Registers the ExceptionMiddleware in the application's request pipeline.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <returns>The updated application builder.</returns>
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }

        /// <summary>
        /// Registers the ApiKeyMiddleware in the application's request pipeline.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <returns>The updated application builder.</returns>
        public static IApplicationBuilder UseApiKeyMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiKeyMiddleware>();
        }
    }
}
