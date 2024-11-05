using System.Net;
using System.Text.Json;

namespace TodoApp.Api.Middleware
{
    /// <summary>
    /// Middleware for handling exceptions globally.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="logger">The logger instance.</param>
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Invokes the middleware to handle exceptions.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns>A task that represents the completion of request processing.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Call the next middleware in the pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the exception details
                _logger.LogError(ex, "An unhandled exception has occurred.");

                // Handle the exception and return a standardized error response
                await HandleExceptionAsync(context);
            }
        }

        /// <summary>
        /// Handles the exception by writing a standardized error response.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns>A task that represents the completion of error handling.</returns>
        private static Task HandleExceptionAsync(HttpContext context)
        {
            // Define the response content type and status code
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Create a standardized error response
            var response = new
            {
                Message = "An unexpected error occurred. Please try again later.",
            };

            // Serialize the response to JSON
            var jsonResponse = JsonSerializer.Serialize(response);

            // Write the response
            return context.Response.WriteAsync(jsonResponse);
        }
    }

}
