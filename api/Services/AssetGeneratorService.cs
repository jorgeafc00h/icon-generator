namespace IconGenerator.Functions.Services;

using System.IO.Compression;
using System.Text;
using System.Text.Json;
using IconGenerator.Functions.Constants;
using IconGenerator.Functions.Models;
using Microsoft.Extensions.Logging;

public class AssetGeneratorService : IAssetGeneratorService
{
    private readonly IImageService _imageService;
    private readonly ILogger<AssetGeneratorService> _logger;

    public AssetGeneratorService(
        IImageService imageService,
        ILogger<AssetGeneratorService> logger)
    {
        _imageService = imageService;
        _logger = logger;
    }

    public async Task<byte[]> GenerateAssetsZipAsync(
        byte[] originalImage,
        AssetGenerationRequest request,
        CancellationToken cancellationToken = default)
    {
        using var zipStream = new MemoryStream();
        using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            // Generate iOS assets
            if (request.Platforms.Contains("ios"))
            {
                await GenerateIOSAssetsAsync(archive, originalImage, request.Options, cancellationToken);
            }

            // Generate Android assets
            if (request.Platforms.Contains("android"))
            {
                await GenerateAndroidAssetsAsync(archive, originalImage, request.Options, cancellationToken);
            }

            // Generate Web assets
            if (request.Platforms.Contains("web"))
            {
                await GenerateWebAssetsAsync(archive, originalImage, request.Options, cancellationToken);
            }

            // Generate macOS assets
            if (request.Platforms.Contains("macos"))
            {
                await GenerateMacOSAssetsAsync(archive, originalImage, request.Options, cancellationToken);
            }
        }

        return zipStream.ToArray();
    }

    private async Task GenerateIOSAssetsAsync(
        ZipArchive archive,
        byte[] originalImage,
        AssetGenerationOptions? options,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating iOS assets");

        var generateAppIconSet = options?.GenerateAppIconSet ?? true;
        var basePath = generateAppIconSet ? "ios/AppIcon.appiconset" : "ios";

        // Generate each iOS size
        foreach (var size in IconSizes.IOS)
        {
            var resizedImage = await _imageService.ResizeImageAsync(
                originalImage,
                size.Width,
                size.Height);

            var fileName = generateAppIconSet
                ? $"{basePath}/{GetIOSAssetCatalogName(size)}.png"
                : $"{basePath}/{size.Name}.png";

            AddToZip(archive, fileName, resizedImage);
        }

        // Generate Contents.json if using asset catalog
        if (generateAppIconSet)
        {
            var contentsJson = GenerateIOSContentsJson();
            var jsonBytes = Encoding.UTF8.GetBytes(contentsJson);
            AddToZip(archive, $"{basePath}/Contents.json", jsonBytes);
        }

        // Add README
        var readme = GetIOSReadme();
        AddToZip(archive, "ios/README.txt", Encoding.UTF8.GetBytes(readme));
    }

    private async Task GenerateAndroidAssetsAsync(
        ZipArchive archive,
        byte[] originalImage,
        AssetGenerationOptions? options,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating Android assets");

        var includeAdaptive = options?.IncludeAdaptiveIcons ?? true;
        var backgroundColor = options?.BackgroundColor ?? "#FFFFFF";

        // Generate standard launcher icons
        foreach (var size in IconSizes.ANDROID)
        {
            var resizedImage = await _imageService.ResizeImageAsync(
                originalImage,
                size.Width,
                size.Height,
                roundCorners: size.Name == "ic_launcher_round");

            var fileName = string.IsNullOrEmpty(size.Folder)
                ? $"android/{size.Name}.png"
                : $"android/{size.Folder}/{size.Name}.png";

            AddToZip(archive, fileName, resizedImage);
        }

        // Generate adaptive icons
        if (includeAdaptive)
        {
            var densities = new[] { ("mdpi", 108), ("hdpi", 162), ("xhdpi", 216), ("xxhdpi", 324), ("xxxhdpi", 432) };

            foreach (var (density, size) in densities)
            {
                // Foreground layer
                var foreground = await _imageService.CreateAdaptiveForegroundAsync(originalImage, size, size);
                AddToZip(archive, $"android/mipmap-{density}/ic_launcher_foreground.png", foreground);

                // Background layer
                var background = await _imageService.CreateAdaptiveBackgroundAsync(size, size, backgroundColor);
                AddToZip(archive, $"android/mipmap-{density}/ic_launcher_background.png", background);
            }

            // Add XML configuration files
            AddAndroidAdaptiveIconXML(archive);
        }

        // Add README
        var readme = GetAndroidReadme();
        AddToZip(archive, "android/README.txt", Encoding.UTF8.GetBytes(readme));
    }

    private async Task GenerateWebAssetsAsync(
        ZipArchive archive,
        byte[] originalImage,
        AssetGenerationOptions? options,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating Web/PWA assets");

        foreach (var size in IconSizes.WEB)
        {
            var resizedImage = await _imageService.ResizeImageAsync(
                originalImage,
                size.Width,
                size.Height);

            AddToZip(archive, $"web/{size.Name}.png", resizedImage);
        }

        // Generate favicon.ico (use 32x32)
        var faviconImage = await _imageService.ResizeImageAsync(originalImage, 32, 32);
        AddToZip(archive, "web/favicon.ico", faviconImage);

        // Generate manifest.json
        var manifest = GenerateWebManifest(options);
        AddToZip(archive, "web/manifest.json", Encoding.UTF8.GetBytes(manifest));

        // Add README
        var readme = GetWebReadme();
        AddToZip(archive, "web/README.txt", Encoding.UTF8.GetBytes(readme));
    }

    private async Task GenerateMacOSAssetsAsync(
        ZipArchive archive,
        byte[] originalImage,
        AssetGenerationOptions? options,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating macOS assets");

        foreach (var size in IconSizes.MACOS)
        {
            var resizedImage = await _imageService.ResizeImageAsync(
                originalImage,
                size.Width,
                size.Height);

            AddToZip(archive, $"macos/{size.Name}.png", resizedImage);
        }

        // Add README
        var readme = GetMacOSReadme();
        AddToZip(archive, "macos/README.txt", Encoding.UTF8.GetBytes(readme));
    }

    private void AddToZip(ZipArchive archive, string entryName, byte[] data)
    {
        var entry = archive.CreateEntry(entryName);
        using var entryStream = entry.Open();
        entryStream.Write(data, 0, data.Length);
    }

    private string GetIOSAssetCatalogName(IconSize size)
    {
        var pts = size.Width / (size.Scale ?? 1);
        return size.Scale == 1
            ? $"icon-{pts}x{pts}"
            : $"icon-{pts}x{pts}@{size.Scale}x";
    }

    private string GenerateIOSContentsJson()
    {
        var contents = new
        {
            images = new[]
            {
                new { size = "20x20", idiom = "iphone", filename = "icon-20x20@2x.png", scale = "2x" },
                new { size = "20x20", idiom = "iphone", filename = "icon-20x20@3x.png", scale = "3x" },
                new { size = "29x29", idiom = "iphone", filename = "icon-29x29@2x.png", scale = "2x" },
                new { size = "29x29", idiom = "iphone", filename = "icon-29x29@3x.png", scale = "3x" },
                new { size = "40x40", idiom = "iphone", filename = "icon-40x40@2x.png", scale = "2x" },
                new { size = "40x40", idiom = "iphone", filename = "icon-40x40@3x.png", scale = "3x" },
                new { size = "60x60", idiom = "iphone", filename = "icon-60x60@2x.png", scale = "2x" },
                new { size = "60x60", idiom = "iphone", filename = "icon-60x60@3x.png", scale = "3x" },
                new { size = "76x76", idiom = "ipad", filename = "icon-76x76.png", scale = "1x" },
                new { size = "76x76", idiom = "ipad", filename = "icon-76x76@2x.png", scale = "2x" },
                new { size = "83.5x83.5", idiom = "ipad", filename = "icon-83.5x83.5@2x.png", scale = "2x" },
                new { size = "1024x1024", idiom = "ios-marketing", filename = "icon-1024x1024.png", scale = "1x" }
            },
            info = new { version = 1, author = "Icon Generator" }
        };

        return JsonSerializer.Serialize(contents, new JsonSerializerOptions { WriteIndented = true });
    }

    private void AddAndroidAdaptiveIconXML(ZipArchive archive)
    {
        var launcherXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<adaptive-icon xmlns:android=""http://schemas.android.com/apk/res/android"">
    <background android:drawable=""@mipmap/ic_launcher_background""/>
    <foreground android:drawable=""@mipmap/ic_launcher_foreground""/>
</adaptive-icon>";

        AddToZip(archive, "android/mipmap-anydpi-v26/ic_launcher.xml", Encoding.UTF8.GetBytes(launcherXml));
        AddToZip(archive, "android/mipmap-anydpi-v26/ic_launcher_round.xml", Encoding.UTF8.GetBytes(launcherXml));
    }

    private string GenerateWebManifest(AssetGenerationOptions? options)
    {
        var manifest = new
        {
            name = options?.AppName ?? "My App",
            short_name = options?.AppName ?? "App",
            icons = new[]
            {
                new { src = "android-chrome-192x192.png", sizes = "192x192", type = "image/png" },
                new { src = "android-chrome-512x512.png", sizes = "512x512", type = "image/png" }
            },
            theme_color = options?.ThemeColor ?? "#ffffff",
            background_color = options?.BackgroundColor ?? "#ffffff",
            display = "standalone"
        };

        return JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true });
    }

    private string GetIOSReadme() => @"iOS App Icons
===============

This package contains all required icon sizes for iOS apps.

INSTALLATION:
1. Open your Xcode project
2. Navigate to Assets.xcassets
3. Drag the AppIcon.appiconset folder into Assets.xcassets
4. Verify icons appear correctly in your target settings

Generated by Icon Generator
";

    private string GetAndroidReadme() => @"Android App Icons
==================

This package contains all required icon sizes for Android apps.

INSTALLATION:
1. Open your Android project
2. Navigate to app/src/main/res/
3. Copy all mipmap-* folders to the res/ directory
4. Verify icons appear in your app

CONTENTS:
- Standard launcher icons (all densities)
- Adaptive icon layers (foreground + background)
- Play Store icon (512x512)

Generated by Icon Generator
";

    private string GetWebReadme() => @"Web/PWA Icons
==============

This package contains all required icons for web apps and PWAs.

INSTALLATION:
Add to your HTML <head>:

<link rel=""icon"" type=""image/x-icon"" href=""/favicon.ico"">
<link rel=""icon"" type=""image/png"" sizes=""32x32"" href=""/favicon-32x32.png"">
<link rel=""icon"" type=""image/png"" sizes=""16x16"" href=""/favicon-16x16.png"">
<link rel=""apple-touch-icon"" sizes=""180x180"" href=""/apple-touch-icon.png"">
<link rel=""manifest"" href=""/manifest.json"">

Generated by Icon Generator
";

    private string GetMacOSReadme() => @"macOS App Icons
================

This package contains icon sizes for macOS apps.

INSTALLATION:
1. Open your Xcode project
2. Navigate to Assets.xcassets
3. Create or select AppIcon
4. Drag PNG files to appropriate size slots

Generated by Icon Generator
";
}
