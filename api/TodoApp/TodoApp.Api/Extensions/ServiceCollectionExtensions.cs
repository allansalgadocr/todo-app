using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TodoApp.Api.Data;
using TodoApp.Api.Mappings;
using TodoApp.Api.Options;
using TodoApp.Api.Repositories;

namespace TodoApp.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            var corsOptions = new ApplicationCorsOptions();
            configuration.Bind("ApplicationCors", corsOptions);

            return services.Configure<ApplicationCorsOptions>(configuration.GetSection("ApplicationCors"))
                           .AddCors(options =>
                           {
                               options.AddPolicy(corsOptions.PolicyName, builder =>
                               {
                                   builder.WithOrigins(corsOptions.AllowedOrigin)
                                          .AllowAnyMethod()
                                          .AllowAnyHeader()
                                          .AllowCredentials();
                               });
                           });
        }

        public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, string connectionString) =>
            services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlite(connectionString));


        public static IServiceCollection AddApplicationRepositories(this IServiceCollection services) =>
            services.AddScoped<ITodoRepository, TodoRepository>();

        public static IServiceCollection AddApplicationAutoMapper(this IServiceCollection services) =>
            services.AddAutoMapper(typeof(TodoProfile).Assembly);

        public static IServiceCollection AddSwaggerWithApiKey(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TodoApp API", Version = "v1" });

                // Define the API Key security scheme
                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description = "API Key needed to access the endpoints.\n\nEnter your API key in the text input below.",
                    In = ParameterLocation.Header,
                    Name = "X-API-KEY",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "ApiKeyScheme"
                });

                // Add the security requirement globally
                var key = new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    },
                    In = ParameterLocation.Header,
                };

                var requirement = new OpenApiSecurityRequirement
                {
                    { key, new List<string>() }
                };

                c.AddSecurityRequirement(requirement);
            });

            return services;
        }
    }
}