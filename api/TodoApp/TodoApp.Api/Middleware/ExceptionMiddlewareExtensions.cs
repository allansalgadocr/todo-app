namespace TodoApp.Api.Middleware
{
    public static class ExceptionMiddlewareExtensions
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
    }
}
