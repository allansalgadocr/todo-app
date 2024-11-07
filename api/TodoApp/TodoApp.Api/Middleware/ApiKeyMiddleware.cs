using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TodoApp.Api.Options;
using System.Collections.Generic;
using System.Linq;

namespace TodoApp.Api.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string APIKEYHEADERNAME = "X-API-KEY";
        private readonly ApiKeyOptions _apiKeyOptions;

        // Define the list of paths to exclude from API Key validation
        private readonly List<string> _excludedPaths = new List<string>
        {
            "/swagger",
            "/swagger/index.html",
            "/swagger/v1/swagger.json",
            "/health"
        };

        public ApiKeyMiddleware(RequestDelegate next, IOptions<ApiKeyOptions> apiKeyOptions)
        {
            _next = next;
            _apiKeyOptions = apiKeyOptions.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                // Allow preflight requests to pass through
                await _next(context);
                return;
            }

            var requestPath = context.Request.Path.Value ?? string.Empty;

            // Check if the request path starts with any of the excluded paths
            if (_excludedPaths.Any(path => requestPath.StartsWith(path, System.StringComparison.OrdinalIgnoreCase)))
            {
                await _next(context);
                return;
            }

            // Check if the API Key header is present
            if (!context.Request.Headers.TryGetValue(APIKEYHEADERNAME, out var extractedApiKey))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("API Key was not provided.");
                return;
            }

            // Validate the API Key
            if (!_apiKeyOptions.ApiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Unauthorized client.");
                return;
            }

            await _next(context);
        }
    }
}
