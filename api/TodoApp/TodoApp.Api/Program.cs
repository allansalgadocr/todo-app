using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TodoApp.Api.Data;
using TodoApp.Api.Extensions;
using TodoApp.Api.Options;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerWithApiKey();

builder.Services.AddApplicationDbContext(configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("No connection string found."));
builder.Services.AddApplicationRepositories();
builder.Services.AddApplicationAutoMapper();
builder.Services.AddApplicationCorsPolicy(configuration);

builder.Services.Configure<ApiKeyOptions>(
    builder.Configuration.GetSection("ApiKeySettings"));

var app = builder.Build();

// Middleware to handle global exceptions and return a JSON response
app.UseExceptionMiddleware();
// Middleware to check for the presence of an API key in the request headers
app.UseApiKeyMiddleware();

// Create a scoped service provider to apply any pending migrations on application startup.
// This ensures the database schema is up-to-date without requiring manual migration steps.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

IOptions<ApplicationCorsOptions> corsOptions = app.Services.GetRequiredService<IOptions<ApplicationCorsOptions>>();
app.UseCors(corsOptions.Value.PolicyName);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
