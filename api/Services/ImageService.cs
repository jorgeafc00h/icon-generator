namespace IconGenerator.Functions.Services;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using Microsoft.Extensions.Logging;

public class ImageService : IImageService
{
    private readonly ILogger<ImageService> _logger;

    public ImageService(ILogger<ImageService> logger)
    {
        _logger = logger;
    }

    public async Task<byte[]> ResizeImageAsync(
        byte[] imageData,
        int width,
        int height,
        bool roundCorners = false,
        string? backgroundColor = null)
    {
        using var image = Image.Load<Rgba32>(imageData);

        // Resize with high quality
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(width, height),
            Mode = ResizeMode.Pad,
            Sampler = KnownResamplers.Lanczos3
        }));

        // Apply rounded corners if requested
        if (roundCorners)
        {
            var cornerRadius = (float)(width * 0.225); // 22.5% radius (Apple standard)
            // TODO: Implement rounded corners with ImageSharp v3 API
            _logger.LogWarning("Rounded corners not yet implemented");
        }

        // Save to byte array
        using var outputStream = new MemoryStream();
        await image.SaveAsPngAsync(outputStream);
        return outputStream.ToArray();
    }

    public async Task<byte[]> CreateAdaptiveForegroundAsync(byte[] imageData, int width, int height)
    {
        // Adaptive icons: 108x108dp with 72x72dp safe zone
        // Scale to 66.67% for safe zone
        const float safeZoneRatio = 72f / 108f;
        var iconSize = (int)(width * safeZoneRatio);
        var padding = (width - iconSize) / 2;

        using var sourceImage = Image.Load<Rgba32>(imageData);

        // Resize source to safe zone size
        sourceImage.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(iconSize, iconSize),
            Mode = ResizeMode.Pad,
            Sampler = KnownResamplers.Lanczos3
        }));

        // Create canvas with transparent background
        using var canvas = new Image<Rgba32>(width, height, Color.Transparent);

        // Place icon in center
        canvas.Mutate(x => x.DrawImage(sourceImage, new Point(padding, padding), 1f));

        using var outputStream = new MemoryStream();
        await canvas.SaveAsPngAsync(outputStream);
        return outputStream.ToArray();
    }

    public async Task<byte[]> CreateAdaptiveBackgroundAsync(int width, int height, string backgroundColor)
    {
        var color = ParseColor(backgroundColor);
        using var image = new Image<Rgba32>(width, height, color);

        using var outputStream = new MemoryStream();
        await image.SaveAsPngAsync(outputStream);
        return outputStream.ToArray();
    }

    private Color ParseColor(string colorString)
    {
        // Remove # if present
        colorString = colorString.TrimStart('#');

        if (colorString.Length == 6)
        {
            // RGB format
            return Color.ParseHex(colorString);
        }
        else if (colorString.Length == 8)
        {
            // RGBA format
            return Color.ParseHex(colorString);
        }

        // Default to white
        return Color.White;
    }
}
