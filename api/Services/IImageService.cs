namespace IconGenerator.Functions.Services;

public interface IImageService
{
    Task<byte[]> ResizeImageAsync(byte[] imageData, int width, int height, bool roundCorners = false, string? backgroundColor = null);
    Task<byte[]> CreateAdaptiveForegroundAsync(byte[] imageData, int width, int height);
    Task<byte[]> CreateAdaptiveBackgroundAsync(int width, int height, string backgroundColor);
}
