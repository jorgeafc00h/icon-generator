using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IconGenerator.Functions.Services;
using IconGenerator.Functions.Options;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var configuration = context.Configuration;

        // Application Insights
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Configuration Options
        services.Configure<AzureOpenAIOptions>(configuration.GetSection("AzureOpenAI"));
        services.Configure<DatabaseOptions>(configuration.GetSection("Database"));
        services.Configure<StorageOptions>(configuration.GetSection("Storage"));
        services.Configure<StripeOptions>(configuration.GetSection("Stripe"));

        // Register Services
        services.AddSingleton<PromptEngineeringService>();
        services.AddSingleton<IAIService, AIService>();
        services.AddSingleton<IStorageService, StorageService>();
        services.AddSingleton<IImageService, ImageService>();
        services.AddSingleton<IAssetGeneratorService, AssetGeneratorService>();

        // Database Service (CosmosDB only for now)
        services.AddSingleton<IDatabaseService, CosmosDbService>();

        // Payment Service
        services.AddSingleton<IPaymentService, StripePaymentService>();

        // HTTP Client
        services.AddHttpClient();
    })
    .Build();

await host.RunAsync();
