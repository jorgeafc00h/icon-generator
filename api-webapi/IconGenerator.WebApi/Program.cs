using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IconGenerator.Functions.Services;
using IconGenerator.Functions.Options;
using IconGenerator.WebApi.Services;
using IconGenerator.WebApi.Options;
using IconGenerator.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var configuration = builder.Configuration;

// Add CORS
var allowedOrigins = new[]
{
    "http://localhost:5173",
    "http://localhost:3000",
    "https://mango-bay-068c07f0f.6.azurestaticapps.net"
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

// Add Controllers
builder.Services.AddControllers();

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
builder.Services.AddSingleton<IAIService, AIService>();
builder.Services.AddSingleton<IStorageService, StorageService>();
builder.Services.AddSingleton<IImageService, ImageService>();
builder.Services.AddSingleton<IAssetGeneratorService, AssetGeneratorService>();
builder.Services.AddSingleton<IDatabaseService, CosmosDbService>();
builder.Services.AddSingleton<IPaymentService, StripePaymentService>();
builder.Services.AddSingleton<IJwtService, JwtService>();

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
