namespace IconGenerator.Functions.Services;

public interface IStorageService
{
    Task<string> UploadImageAsync(string imageUrl, string userId, string iconId, CancellationToken cancellationToken = default);
    Task<string> UploadImageAsync(byte[] imageData, string userId, string iconId, CancellationToken cancellationToken = default);
    Task<byte[]> DownloadImageAsync(string imageUrl, CancellationToken cancellationToken = default);
    Task<string> UploadZipAsync(byte[] zipData, string userId, string fileName, CancellationToken cancellationToken = default);
    Task<string?> GetImageUrlAsync(string userId, string iconId, CancellationToken cancellationToken = default);
}
