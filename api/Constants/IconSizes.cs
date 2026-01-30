namespace IconGenerator.Functions.Constants;

public record IconSize(string Name, int Width, int Height, int? Scale = null, string? Density = null, string? Folder = null);

public static class IconSizes
{
    // iOS Icon Sizes
    public static readonly List<IconSize> IOS = new()
    {
        // iPhone App Icon
        new("icon-60@2x", 120, 120, 2),
        new("icon-60@3x", 180, 180, 3),

        // iPad App Icon
        new("icon-76", 76, 76, 1),
        new("icon-76@2x", 152, 152, 2),
        new("icon-83.5@2x", 167, 167, 2),

        // Spotlight
        new("icon-40@2x", 80, 80, 2),
        new("icon-40@3x", 120, 120, 3),

        // Settings
        new("icon-29@2x", 58, 58, 2),
        new("icon-29@3x", 87, 87, 3),

        // Notifications
        new("icon-20@2x", 40, 40, 2),
        new("icon-20@3x", 60, 60, 3),

        // App Store
        new("icon-1024", 1024, 1024, 1),
    };

    // Android Icon Sizes
    public static readonly List<IconSize> ANDROID = new()
    {
        new("ic_launcher", 48, 48, Density: "mdpi", Folder: "mipmap-mdpi"),
        new("ic_launcher", 72, 72, Density: "hdpi", Folder: "mipmap-hdpi"),
        new("ic_launcher", 96, 96, Density: "xhdpi", Folder: "mipmap-xhdpi"),
        new("ic_launcher", 144, 144, Density: "xxhdpi", Folder: "mipmap-xxhdpi"),
        new("ic_launcher", 192, 192, Density: "xxxhdpi", Folder: "mipmap-xxxhdpi"),

        // Round icons
        new("ic_launcher_round", 48, 48, Density: "mdpi", Folder: "mipmap-mdpi"),
        new("ic_launcher_round", 72, 72, Density: "hdpi", Folder: "mipmap-hdpi"),
        new("ic_launcher_round", 96, 96, Density: "xhdpi", Folder: "mipmap-xhdpi"),
        new("ic_launcher_round", 144, 144, Density: "xxhdpi", Folder: "mipmap-xxhdpi"),
        new("ic_launcher_round", 192, 192, Density: "xxxhdpi", Folder: "mipmap-xxxhdpi"),

        // Play Store
        new("playstore-icon", 512, 512),
    };

    // Web/PWA Icon Sizes
    public static readonly List<IconSize> WEB = new()
    {
        new("favicon-16x16", 16, 16),
        new("favicon-32x32", 32, 32),
        new("favicon-48x48", 48, 48),
        new("apple-touch-icon", 180, 180),
        new("android-chrome-192x192", 192, 192),
        new("android-chrome-512x512", 512, 512),
    };

    // macOS Icon Sizes
    public static readonly List<IconSize> MACOS = new()
    {
        new("icon_16x16", 16, 16),
        new("icon_16x16@2x", 32, 32),
        new("icon_32x32", 32, 32),
        new("icon_32x32@2x", 64, 64),
        new("icon_128x128", 128, 128),
        new("icon_128x128@2x", 256, 256),
        new("icon_256x256", 256, 256),
        new("icon_256x256@2x", 512, 512),
        new("icon_512x512", 512, 512),
        new("icon_512x512@2x", 1024, 1024),
    };
}
