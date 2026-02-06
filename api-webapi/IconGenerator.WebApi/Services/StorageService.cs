namespace IconGenerator.Functions.Services;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using IconGenerator.Functions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class StorageService : IStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;
    private readonly string _assetsContainerName;
    private readonly ILogger<StorageService> _logger;
    private readonly HttpClient _httpClient;

    public StorageService(
        IOptions<StorageOptions> options,
        IHttpClientFactory httpClientFactory,
        ILogger<StorageService> logger)
    {
        _logger = logger;
        var storageOptions = options.Value;

        _blobServiceClient = new BlobServiceClient(storageOptions.ConnectionString);
        _containerName = storageOptions.ContainerName;
        _assetsContainerName = storageOptions.AssetsContainerName;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<string> UploadImageAsync(
        string imageUrl,
        string userId,
        string iconId,
        CancellationToken cancellationToken = default)
    {
        // Download image from URL
        var imageData = await _httpClient.GetByteArrayAsync(imageUrl, cancellationToken);

        return await UploadImageAsync(imageData, userId, iconId, cancellationToken);
    }

    public async Task<string> UploadImageAsync(
        byte[] imageData,
        string userId,
        string iconId,
        CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        // Ensure container exists with private access
        await containerClient.CreateIfNotExistsAsync(
            PublicAccessType.None,
            cancellationToken: cancellationToken);

        // Upload blob
        var blobName = $"{userId}/{iconId}.png";
        var blobClient = containerClient.GetBlobClient(blobName);

        var blobHttpHeaders = new BlobHttpHeaders
        {
            ContentType = "image/png",
            CacheControl = "public, max-age=31536000" // Cache for 1 year
        };

        using var stream = new MemoryStream(imageData);
        await blobClient.UploadAsync(
            stream,
            new BlobUploadOptions
            {
                HttpHeaders = blobHttpHeaders
            },
            cancellationToken);

        _logger.LogInformation("Uploaded image to: {BlobName}", blobName);

        // Generate SAS token for read access (valid for 1 year)
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _containerName,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddYears(1)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasUri = blobClient.GenerateSasUri(sasBuilder);
        return sasUri.ToString();
    }

    public async Task<byte[]> DownloadImageAsync(
        string imageUrl,
        CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetByteArrayAsync(imageUrl, cancellationToken);
    }

    public async Task<string> UploadZipAsync(
        byte[] zipData,
        string userId,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_assetsContainerName);

        // Ensure container exists with private access
        await containerClient.CreateIfNotExistsAsync(
            PublicAccessType.None,
            cancellationToken: cancellationToken);

        // Upload blob
        var blobName = $"{userId}/{fileName}";
        var blobClient = containerClient.GetBlobClient(blobName);

        var blobHttpHeaders = new BlobHttpHeaders
        {
            ContentType = "application/zip",
            CacheControl = "public, max-age=604800" // Cache for 7 days
        };

        using var stream = new MemoryStream(zipData);
        await blobClient.UploadAsync(
            stream,
            new BlobUploadOptions
            {
                HttpHeaders = blobHttpHeaders
            },
            cancellationToken);

        _logger.LogInformation("Uploaded ZIP to: {BlobName}", blobName);

        // Generate SAS token for read access (valid for 7 days)
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _assetsContainerName,
            BlobName = blobName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddDays(7)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);

        var sasUri = blobClient.GenerateSasUri(sasBuilder);
        return sasUri.ToString();
    }
}
