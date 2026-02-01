using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IconGenerator.Functions.Services;
using IconGenerator.Functions.Models;
using IconGenerator.Functions.Options;
using Xunit;

namespace IconGenerator.Tests;

/// <summary>
/// xUnit test fixture for dependency injection setup
/// </summary>
public class TestFixture : IDisposable
{
    public IServiceProvider ServiceProvider { get; }
    public IConfiguration Configuration { get; }

    public TestFixture()
    {
        // Load environment variables from .env file
        DotNetEnv.Env.Load();

        // Build configuration
        Configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        // Setup dependency injection
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Configure Azure OpenAI options
        services.Configure<AzureOpenAIOptions>(options =>
        {
            options.Endpoint = Configuration["AZURE_OPENAI_ENDPOINT"]
                ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT not set");
            options.ApiKey = Configuration["AZURE_OPENAI_API_KEY"]
                ?? throw new InvalidOperationException("AZURE_OPENAI_API_KEY not set");
            options.DallE3Deployment = Configuration["DALLE3_DEPLOYMENT_NAME"] ?? "dall-e-3";
            options.Gpt4oMiniDeployment = Configuration["GPT4O_MINI_DEPLOYMENT_NAME"] ?? "gpt-4o-mini";
        });

        // Configure Storage options
        services.Configure<StorageOptions>(options =>
        {
            options.ConnectionString = Configuration["AZURE_STORAGE_CONNECTION_STRING"]
                ?? throw new InvalidOperationException("AZURE_STORAGE_CONNECTION_STRING not set");
            options.ContainerName = Configuration["STORAGE_CONTAINER_NAME"] ?? "generated-icons";
            options.AssetsContainerName = Configuration["STORAGE_ASSETS_CONTAINER_NAME"] ?? "app-resources";
        });

        // Add HttpClient factory
        services.AddHttpClient();

        // Register services
        services.AddSingleton<PromptEngineeringService>();
        services.AddSingleton<AIService>();
        services.AddSingleton<IImageService, ImageService>();
        services.AddSingleton<IStorageService, StorageService>();

        ServiceProvider = services.BuildServiceProvider();
    }

    public void Dispose()
    {
        if (ServiceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}

/// <summary>
/// Collection definition for shared test fixture
/// </summary>
[CollectionDefinition("Integration Tests")]
public class IntegrationTestCollection : ICollectionFixture<TestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
