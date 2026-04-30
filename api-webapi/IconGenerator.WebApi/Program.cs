using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IconGenerator.Functions.Services;
using IconGenerator.Functions.Options;
using IconGenerator.WebApi.Services;
using IconGenerator.WebApi.Options;
using IconGenerator.WebApi.Middleware;
using IconGenerator.WebApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var configuration = builder.Configuration;

// Add CORS
var allowedOrigins = new[]
{
    "http://localhost:5173",
    "http://localhost:3000",
    "https://mango-bay-068c07f0f.6.azurestaticapps.net",
    "https://blue-bush-05743730f.7.azurestaticapps.net"
};

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add Controllers with JSON options for enum string handling
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Allow enums to be sent as strings instead of integers
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Configure JWT Authentication
var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
if (jwtOptions == null || string.IsNullOrEmpty(jwtOptions.SecretKey))
{
    throw new InvalidOperationException("JWT configuration is missing or invalid. Please configure Jwt:SecretKey in appsettings.json or environment variables.");
}

builder.Services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
        ClockSkew = TimeSpan.FromMinutes(5)
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("Authentication failed: {Error}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            var userId = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            logger.LogInformation("Token validated for user: {UserId}", userId);
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Configure Application Insights
builder.Services.AddApplicationInsightsTelemetry();

// Configure Options
builder.Services.Configure<AppSettingsOptions>(configuration.GetSection("AppSettings"));
builder.Services.Configure<AzureOpenAIOptions>(configuration.GetSection("AzureOpenAI"));
builder.Services.Configure<DatabaseOptions>(configuration.GetSection("Database"));
builder.Services.Configure<StorageOptions>(configuration.GetSection("Storage"));
builder.Services.Configure<StripeOptions>(configuration.GetSection("Stripe"));

// Register Services
builder.Services.AddSingleton<PromptEngineeringService>();
builder.Services.AddSingleton<UIPromptEngineeringService>();
builder.Services.AddSingleton<IAIService, AIService>();
builder.Services.AddSingleton<IStorageService, StorageService>();
builder.Services.AddSingleton<IImageService, ImageService>();
builder.Services.AddSingleton<IAssetGeneratorService, AssetGeneratorService>();
builder.Services.AddScoped<IPaymentService, StripePaymentService>();
builder.Services.AddSingleton<IJwtService, JwtService>();

// Database service registration by provider type
var databaseOptions = configuration.GetSection("Database").Get<DatabaseOptions>() ?? new DatabaseOptions();
if (databaseOptions.Type.Equals("sqlite", StringComparison.OrdinalIgnoreCase))
{
    var sqliteConnectionString = string.IsNullOrWhiteSpace(databaseOptions.SqliteConnectionString)
        ? "Data Source=./data/icon-generator.db"
        : databaseOptions.SqliteConnectionString;

    var sqlitePath = ExtractSqlitePath(sqliteConnectionString);
    if (!string.IsNullOrWhiteSpace(sqlitePath))
    {
        var directory = Path.GetDirectoryName(sqlitePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(sqliteConnectionString));
    builder.Services.AddScoped<IDatabaseService, SqliteDbService>();
}
else
{
    builder.Services.AddSingleton<IDatabaseService, CosmosDbService>();
}

static string? ExtractSqlitePath(string connectionString)
{
    const string key = "Data Source=";
    var index = connectionString.IndexOf(key, StringComparison.OrdinalIgnoreCase);
    if (index < 0)
    {
        return null;
    }

    var value = connectionString[(index + key.Length)..];
    var semicolonIndex = value.IndexOf(';');
    if (semicolonIndex >= 0)
    {
        value = value[..semicolonIndex];
    }

    return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

// HTTP Client
builder.Services.AddHttpClient();

// Health Checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure Middleware Pipeline

// Exception Handling (must be first)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// HTTPS Redirection
app.UseHttpsRedirection();

// CORS (must be before authentication)
app.UseCors();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

// Health Check Endpoint
app.MapHealthChecks("/health");

// Log startup information
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Icon Generator Web API starting up");
logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
logger.LogInformation("Allowed CORS Origins: {Origins}", string.Join(", ", allowedOrigins));

app.Run();
