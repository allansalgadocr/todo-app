using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Exceptions;

namespace TodoApp.Api.Middleware
{
    /// <summary>
    /// Middleware for handling exceptions globally.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="logger">The logger instance.</param>
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (UserFriendlyException ex)
            {
                // Log the exception details as a warning
                _logger.LogWarning(ex, "A user-friendly exception occurred.");

                // Handle the exception and return a standardized error response
                await HandleExceptionAsync(context, ex.Message, ex.StatusCode);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Log the concurrency exception as an error
                _logger.LogError(ex, "A concurrency error occurred.");

                // Return a 409 Conflict response with a user-friendly message
                await HandleExceptionAsync(context, "A concurrency error occurred while processing your request.", StatusCodes.Status409Conflict);
            }
            catch (DbUpdateException ex)
            {
                // Log the database update exception as an error
                _logger.LogError(ex, "A database update error occurred.");

                // Return a 500 Internal Server Error response with a user-friendly message
                await HandleExceptionAsync(context, "A database error occurred while processing your request.", StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                // Log any other unhandled exceptions as errors
                _logger.LogError(ex, "An unexpected error occurred.");

                // Return a 500 Internal Server Error response with a generic user-friendly message
                await HandleExceptionAsync(context, "An unexpected error occurred. Please try again later.", StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Handles the exception by writing a standardized error response.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <param name="message">The error message to return.</param>
        /// <param name="statusCode">The HTTP status code to set.</param>
        /// <returns>A task that represents the completion of error handling.</returns>
        private static Task HandleExceptionAsync(HttpContext context, string message, int statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new
            {
                Message = message
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var jsonResponse = JsonSerializer.Serialize(response, options);

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
