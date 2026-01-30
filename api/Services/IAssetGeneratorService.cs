namespace IconGenerator.Functions.Services;

using IconGenerator.Functions.Models;

public interface IAssetGeneratorService
{
    Task<byte[]> GenerateAssetsZipAsync(
        byte[] originalImage,
        AssetGenerationRequest request,
        CancellationToken cancellationToken = default);
}
