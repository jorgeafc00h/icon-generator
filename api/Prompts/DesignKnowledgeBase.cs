namespace IconGenerator.Functions.Prompts;

/// <summary>
/// Design knowledge base containing UI/UX principles, color theory, and composition guidelines
/// for generating high-quality app icons and UI screens
/// </summary>
public static class DesignKnowledgeBase
{
    /// <summary>
    /// Core UI/UX design principles for app icons
    /// </summary>
    public const string IconDesignPrinciples = @"
CORE DESIGN PRINCIPLES FOR APP ICONS:

1. CLARITY & SIMPLICITY
   - Icons should be immediately recognizable at small sizes (29x29px to 1024x1024px)
   - Use simple, bold shapes that read well at any scale
   - Avoid fine details, thin lines, or intricate patterns
   - One clear focal point - don't try to communicate multiple concepts

2. VISUAL HIERARCHY
   - Primary element should occupy 60-70% of the icon space
   - Use the rule of thirds for positioning key elements
   - Maintain clear figure-ground relationship
   - Ensure adequate negative space (breathing room)

3. CONSISTENCY
   - Maintain consistent visual weight across icon sets
   - Use consistent stroke weights (typically 2-4% of icon size)
   - Keep corner radius consistent (iOS: 22.5% of icon size)
   - Uniform lighting direction (top-left at 45°)

4. PLATFORM GUIDELINES
   iOS:
   - Rounded corners (22.5% radius) applied by system
   - Avoid placing crucial content in corners
   - Use depth and realism sparingly
   - Prefer rich, saturated colors

   Android (Material Design):
   - Can use transparency in adaptive icons
   - 72dp safe zone within 108dp canvas
   - Use shadow and elevation principles
   - Follow Material color system

5. COLOR THEORY
   - Maximum 3-4 colors for clarity
   - Use 60-30-10 rule (dominant, secondary, accent)
   - Ensure sufficient contrast (WCAG AA: 4.5:1 minimum)
   - Consider colorblind accessibility
   - Test in both light and dark modes

6. DEPTH & DIMENSION
   - Use gradients to create subtle depth
   - Apply consistent light source
   - Shadow should be subtle (20-30% opacity)
   - Highlight on top-left, shadow on bottom-right

7. UNIQUENESS
   - Avoid clichés and overused metaphors
   - Don't copy existing popular apps
   - Create distinctive silhouette
   - Use unexpected color combinations when appropriate
";

    /// <summary>
    /// UI Screen design principles
    /// </summary>
    public const string ScreenDesignPrinciples = @"
CORE DESIGN PRINCIPLES FOR UI SCREENS:

1. VISUAL HIERARCHY
   - F-pattern or Z-pattern for content flow
   - Clear information architecture
   - Prominent CTAs (Call-to-Action)
   - Consistent heading sizes (H1: 32-48px, H2: 24-32px, H3: 18-24px)

2. SPACING & LAYOUT
   - Use 8px grid system (all spacing multiples of 8)
   - Consistent padding (16px, 24px, 32px)
   - White space is essential - don't crowd
   - Maximum 60-80 characters per line for readability

3. COLOR SYSTEM
   - Primary color: Brand identity, CTAs
   - Secondary color: Supporting actions
   - Neutral palette: Backgrounds, text, borders
   - Semantic colors: Success (green), Error (red), Warning (yellow)
   - 8-10 color palette maximum

4. TYPOGRAPHY
   - Maximum 2-3 font families
   - Clear hierarchy with size, weight, and spacing
   - Body text: 16px minimum for mobile
   - Adequate line height (1.4-1.6 for body text)

5. MODERN UI PATTERNS
   - Cards for grouped content
   - Bottom sheets for mobile actions
   - Floating action buttons (FAB) for primary action
   - Top navigation or tabs for sections
   - Search prominently placed

6. ACCESSIBILITY
   - Minimum touch target: 44x44px (iOS), 48x48dp (Android)
   - Color contrast ratios (WCAG AA)
   - Clear focus states
   - Readable font sizes
";

    /// <summary>
    /// Style-specific prompt enhancements
    /// </summary>
    public static class StyleTemplates
    {
        public const string Modern3D = @"
3D STYLE GUIDELINES:
- Use soft, rounded geometric shapes with depth
- Apply realistic lighting: key light from top-left at 45°, subtle rim light
- Gradient overlays for dimension (lighter at top, darker at bottom)
- Subtle drop shadow (20-30% opacity, 2-4px blur)
- Smooth surfaces with subtle reflections
- Isometric or slight perspective view
- Depth should enhance, not overwhelm
- Use subsurface scattering for organic materials
";

        public const string Minimal = @"
MINIMAL STYLE GUIDELINES:
- Pure geometric shapes (circles, squares, triangles)
- Flat colors, no gradients
- Maximum 2-3 colors
- Generous negative space (40-50% of canvas)
- Thin, consistent line weights (1-2px)
- Perfect alignment and symmetry
- Swiss design principles
- Subtract until you can't anymore
";

        public const string Gradient = @"
GRADIENT STYLE GUIDELINES:
- Smooth, multi-point gradients (3-4 color stops)
- Use color harmony (analogous, triadic, or complementary)
- Mesh gradients for organic flow
- Avoid harsh transitions
- Apply to both shapes and backgrounds
- Consider gradient direction (diagonal often best)
- Modern, vibrant color combinations
- Instagram/Asana gradient inspiration
";

        public const string Glassmorphism = @"
GLASSMORPHISM STYLE GUIDELINES:
- Frosted glass effect with blur
- Subtle transparency (10-30% opacity)
- Light border (1px, 20-40% white opacity)
- Layered depth with multiple glass panels
- Bright, vibrant background colors
- Soft shadows between layers
- iOS 7+ inspiration
";

        public const string Neomorphism = @"
NEOMORPHISM (SOFT UI) GUIDELINES:
- Monochromatic color scheme
- Extruded shapes from background
- Two shadows: dark bottom-right, light top-left
- Subtle gradients matching background
- Low contrast (accessibility concern - use sparingly)
- Soft, rounded corners
- Minimal color variation
";

        public const string Claymorphism = @"
CLAY 3D RENDER STYLE (PROFESSIONAL STUDIO QUALITY):
- High-quality 3D clay render with smooth, matte finish
- Soft plasticine/modeling clay texture with zero shininess
- Professionally lit: soft studio lighting from multiple angles
- Rounded, puffy organic shapes with thick, solid volumes
- Rich, saturated colors (vivid but not neon - think real clay colors)
- Smooth beveled edges and soft transitions between forms
- Subtle ambient occlusion in crevices and contact points
- Clean, professional product photography aesthetic
- Centered on neutral gradient background (light gray to white)
- Multiple layers or stacked elements for depth
- Slightly exaggerated, friendly proportions
- Perfect for: food items, objects, characters, everyday items
- Reference: Behance/Dribbble clay renders, studio product shots
- NO text, NO labels, NO flat illustrations
- Think: high-end toy design or advertising product render
";

        public const string Pixel = @"
PIXEL ART STYLE GUIDELINES:
- 8-bit or 16-bit aesthetic
- Limited color palette (8-32 colors)
- Visible pixels, no anti-aliasing
- Dithering for gradients
- Strong outlines (1-2px black)
- Retro gaming inspiration
- Isometric or side view
- Nostalgia factor
";
    }

    /// <summary>
    /// Professional color palettes
    /// </summary>
    public static class ColorPalettes
    {
        public static readonly Dictionary<string, (string[] Colors, string Description)> Palettes = new()
        {
            ["tech-blue"] = (
                new[] { "#0066FF", "#00D4FF", "#FFFFFF", "#F0F4F8" },
                "Professional, trustworthy, SaaS/Tech - primary: vibrant blue, accent: cyan, neutral: whites/grays"
            ),
            ["vibrant-purple"] = (
                new[] { "#6C5CE7", "#A29BFE", "#FD79A8", "#FDCB6E" },
                "Creative, innovative, Design tools - purple to pink gradient with warm accent"
            ),
            ["organic-green"] = (
                new[] { "#00B894", "#00CEC9", "#55EFC4", "#81ECEC" },
                "Health, wellness, sustainability - fresh greens and teals"
            ),
            ["warm-sunset"] = (
                new[] { "#FF6B6B", "#FF8E53", "#FFBB5C", "#FFE66D" },
                "Energy, food, lifestyle - warm oranges and yellows"
            ),
            ["professional-gray"] = (
                new[] { "#2D3436", "#636E72", "#B2BEC3", "#DFE6E9" },
                "Corporate, finance, professional - sophisticated gray scale"
            ),
            ["playful-candy"] = (
                new[] { "#FF6B9D", "#C44569", "#FFA502", "#F8B500" },
                "Gaming, entertainment, social - fun pinks and oranges"
            ),
            ["minimal-mono"] = (
                new[] { "#000000", "#333333", "#FFFFFF", "#F5F5F5" },
                "Luxury, minimalist, premium - monochrome with high contrast"
            ),
            ["nature-earth"] = (
                new[] { "#6C5B7B", "#C06C84", "#F67280", "#F8B195" },
                "Wellness, organic, natural - earth tones with pink accent"
            ),
            ["ocean-depth"] = (
                new[] { "#0A3D62", "#1289A7", "#12CBC4", "#B8E6E6" },
                "Travel, water, depth - deep blues to light aqua"
            ),
            ["neon-cyber"] = (
                new[] { "#FF10F0", "#00F5FF", "#39FF14", "#FFFF00" },
                "Gaming, tech, futuristic - vibrant neon colors"
            )
        };

        /// <summary>
        /// Get harmonious color palette based on user input or default
        /// </summary>
        public static string GetPalettePrompt(List<string> userColors)
        {
            if (userColors == null || userColors.Count == 0)
            {
                return "Use a harmonious, professional color palette suitable for modern app design.";
            }

            if (userColors.Count == 1)
            {
                return $"Use {userColors[0]} as the primary color with complementary accent colors.";
            }

            return $"Use the following color palette: {string.Join(", ", userColors)}. Ensure harmonious color relationships.";
        }
    }

    /// <summary>
    /// Composition and layout guidelines
    /// </summary>
    public const string CompositionRules = @"
COMPOSITION GUIDELINES:

1. RULE OF THIRDS
   - Place key elements at intersection points
   - Avoid centering everything
   - Create visual tension and interest

2. GOLDEN RATIO (1.618:1)
   - Use for proportions and spacing
   - Creates naturally pleasing compositions

3. VISUAL WEIGHT
   - Larger elements = more weight
   - Bright colors = more weight
   - Complex shapes = more weight
   - Balance weight across canvas

4. DIRECTION & FLOW
   - Eyes follow lines and curves
   - Use directional cues (arrows, gazes, shapes)
   - Create visual path for viewer

5. UNITY & HARMONY
   - Repeat shapes, colors, or patterns
   - Create family resemblance
   - Consistent style throughout
";
}
