using IconGenerator.Functions.Models;
using IconGenerator.Functions.Prompts;

namespace IconGenerator.Functions.Services;

/// <summary>
/// Specialized prompt engineering service for UI/UX screen mockup generation.
/// Handles Material Design 3, iOS HIG, industry patterns, and accessibility guidelines.
/// </summary>
public class UIPromptEngineeringService
{
    /// <summary>
    /// Build comprehensive system prompt for UI generation
    /// </summary>
    public string BuildUISystemPrompt(ScreenType screenType, string category, string platform)
    {
        var componentLibrary = GetComponentLibrary(platform);
        var industryPattern = GetIndustryPattern(category);
        var screenTemplate = UIDesignKnowledgeBase.GetScreenTemplate(screenType);
        var spacingSystem = UIDesignKnowledgeBase.SpacingSystem.GridSystem;
        var accessibility = UIDesignKnowledgeBase.AccessibilityGuidelines;

        return $@"You are an expert UI/UX designer creating professional mobile app screen mockups.

TARGET PLATFORM: {platform}

{componentLibrary}

{industryPattern}

{screenTemplate}

{spacingSystem}

{accessibility}

QUALITY REQUIREMENTS:
- Professional, polished design matching industry standards
- Precise component sizing and spacing using 8px grid
- Clear visual hierarchy with proper typography scale
- Accessible design meeting WCAG AA standards
- Platform-appropriate components and patterns
- Industry-specific patterns and conventions
- Modern, clean aesthetic suitable for app stores

OUTPUT FORMAT:
Generate a detailed, specific UI screen mockup prompt that describes:
1. Exact component specifications (sizes, colors, positions)
2. Layout structure using platform conventions
3. Spacing following 8px grid system
4. Typography hierarchy from platform guidelines
5. Color system appropriate for the app category
6. Navigation pattern suitable for the category
7. Accessibility considerations (touch targets, contrast)

Be specific and precise. Include exact measurements, component types, and visual details.";
    }

    /// <summary>
    /// Build user prompt from app generation request
    /// </summary>
    public string BuildUIUserPrompt(AppResourcesGenerationRequest request)
    {
        var screenType = request.Options.ScreenTypes.FirstOrDefault();
        var platform = request.Options.TargetPlatform?.ToString() ?? request.Platforms.FirstOrDefault() ?? "iOS";
        var category = request.Options.AppCategory ?? "App";
        var appName = request.Options.AppName ?? "MyApp";
        var primaryColor = request.Options.BrandPrimaryColor;
        var secondaryColor = request.Options.BrandSecondaryColor;

        var components = GetUIComponentsForScreen(screenType, category);
        var navigation = GetNavigationPattern(category, screenType);
        var layout = GetLayoutPattern(screenType);
        var spacing = GetSpacingGuidance(platform);

        var colorGuidance = string.Empty;
        if (!string.IsNullOrEmpty(primaryColor))
        {
            colorGuidance = $"Use {primaryColor} as primary brand color";
            if (!string.IsNullOrEmpty(secondaryColor))
            {
                colorGuidance += $" and {secondaryColor} as secondary color";
            }
            colorGuidance += ". ";
        }

        return $@"Create a professional {platform} {screenType} screen mockup for ""{appName}"", a {category} app.

SCREEN TYPE: {screenType}
APP CATEGORY: {category}
PLATFORM: {platform}

REQUIRED COMPONENTS:
{components}

NAVIGATION:
{navigation}

LAYOUT:
{layout}

SPACING & SIZING:
{spacing}

COLORS:
{colorGuidance}Use platform-appropriate color system. Ensure sufficient contrast for accessibility (WCAG AA: 4.5:1 minimum).

QUALITY STANDARDS:
- Follow {platform} design guidelines strictly
- Use 8px grid system for all spacing and sizing
- Ensure touch targets are minimum {(platform == "iOS" ? "44x44px" : "48x48dp")}
- Apply appropriate {category} industry patterns
- Create professional, app-store quality mockup
- Include realistic placeholder content (names, dates, numbers)
- Show visual hierarchy through size, weight, color
- Maintain clean, modern aesthetic

Generate a detailed prompt for creating this screen mockup.";
    }

    /// <summary>
    /// Get comprehensive component list for a screen type
    /// </summary>
    public string GetUIComponentsForScreen(ScreenType screenType, string category)
    {
        var baseComponents = GetBaseScreenComponents(screenType);
        var categorySpecific = GetCategorySpecificComponents(screenType, category);

        if (string.IsNullOrEmpty(categorySpecific))
        {
            return baseComponents;
        }

        return $"{baseComponents}\n\nCATEGORY-SPECIFIC ADDITIONS:\n{categorySpecific}";
    }

    /// <summary>
    /// Get base components for each screen type
    /// </summary>
    private string GetBaseScreenComponents(ScreenType screenType)
    {
        return screenType switch
        {
            ScreenType.Login => @"
- App logo: 80x80px centered, top 20% of screen
- App name: 24sp/pt below logo
- Email text field: 56px height, envelope icon (24x24), 'Email' floating label
- Password text field: 56px height, lock icon (24x24), 'Password' label, eye icon toggle
- Forgot password link: 14sp/pt, right-aligned below password field
- Sign In button: 48px height, full-width with 16px margins, primary color
- Divider: 'or continue with' text with horizontal lines, 32px spacing
- Social auth buttons: Google + Apple side-by-side, 48px height, official brand styling
- Sign up link: 'Don't have an account? Sign up' centered at bottom
- Security badge: Lock icon + 'Secure login' text, subtle, bottom corner",

            ScreenType.Signup => @"
- Header: 'Create Account' 28sp/pt bold
- Full name field: 56px height, person icon (24x24)
- Email field: 56px height, envelope icon (24x24)
- Password field: 56px height, lock icon, password strength indicator bar
- Confirm password field: 56px height, lock icon
- Terms checkbox: Small checkbox + 'I agree to Terms & Privacy Policy' with tappable links
- Sign Up button: 48px height, primary color, full-width, disabled state until form valid
- Social signup: Google + Apple buttons, 48px height each
- Login link: 'Already have an account? Log in' centered at bottom",

            ScreenType.Home => @"
- Navigation: Bottom tab bar (80dp/49pt) with 4-5 items OR top app bar with menu
- Header section: App logo/name, notification bell icon (24x24), user avatar (40px circle)
- Search bar: 48px height, search icon left, placeholder text, rounded corners
- Hero section: Featured content card or carousel, full-width or with margins
- Section header: 'Featured' or category name (18sp/pt bold) with 'See All' link
- Content grid/list: 2-column grid or vertical list of cards
- Card components: Image thumbnail, title (16sp/pt), subtitle (14sp/pt), metadata
- Floating Action Button (FAB): 56x56px, bottom-right, primary color, + or relevant icon",

            ScreenType.Dashboard => @"
- Top bar: 64px height, greeting text 'Good morning, [Name]' (20sp/pt), notification icon, avatar (40px)
- Stat cards row: 3-4 cards in horizontal scroll or grid
  - Large number: 32sp/pt bold (key metric)
  - Label: 14sp/pt below number
  - Icon: 24x24px, colored
  - Trend indicator: ↑/↓ arrow with percentage, colored (green/red)
- Chart section: Line or bar chart with Y-axis labels, period selector tabs (Day/Week/Month/Year)
- Recent activity header: 18sp/pt bold 'Recent Activity' with 'View All' link
- Activity list: 4-5 items, icon (24x24) + text + timestamp + chevron
- Bottom navigation: 4-5 tabs, icons + labels, current highlighted",

            ScreenType.Profile => @"
- Cover photo (optional): 16:9 aspect ratio header image with gradient overlay
- Profile avatar: 96-120px circle, centered or left-aligned, edit button overlay (camera icon)
- Name: 24sp/pt bold below/beside avatar
- Bio/subtitle: 14sp/pt gray, 2-3 lines max
- Stats row: Equal-width columns for metrics (Followers, Following, Posts, etc.)
- Action button: 'Edit Profile' or 'Follow' button, 40-48px height, full or auto width
- Information cards: Email, Phone, Location rows with icons (24x24) and labels
- Settings list: Account, Privacy, Notifications, Help with chevrons (>)
- Sign Out button: Red text button at bottom with warning color",

            ScreenType.Settings => @"
- Top bar: 'Settings' title (20sp/pt), back button (< or arrow)
- Search bar (optional): For long settings lists, 48px height
- Section headers: 'Account', 'Privacy', 'Notifications' (14sp/pt, gray, uppercase/bold)
- Setting items: 56-64px height, icon (24x24) left, label (16sp/pt), chevron (>) right
- Toggle switches: Right-aligned for boolean settings (On/Off states)
- Dividers: 1px hairline between sections or items
- Destructive actions: 'Delete Account', 'Sign Out' in warning red color at bottom
- Version info: 'Version 1.0.0' in small gray text (12sp/pt) at very bottom",

            ScreenType.Onboarding => @"
- Illustration: Large (60% screen height), centered, colorful, relevant to feature
- Headline: 28-32sp/pt bold, 1-2 words, benefit-focused
- Description: 16sp/pt, 2-3 lines explaining feature/benefit, centered
- Page indicators: Dot pagination (screen 1 of 3), horizontal, centered
- Next button: 48px height, primary color, 'Next' label, bottom-right with margin
- Skip button: 'Skip' text button (no background), top-right corner
- Final screen: 'Get Started' button instead of Next
- Swipe hint: Subtle indication of horizontal swipe capability",

            ScreenType.ProductList => @"
- Search bar: 48px height, search icon, 'Search products' placeholder, filter icon right
- Filter chips: Horizontal scrollable row, category tags, selected state indication
- Sort dropdown/button: 'Sort by: Price' with chevron, opens bottom sheet
- View toggle: Grid/List icons (24x24), top-right corner
- Product grid: 2 columns mobile, 3-4 tablet/desktop, 16px gutters
- Product card components:
  - Product image: Square aspect ratio, high quality
  - Price: 20-24sp/pt bold, prominent
  - Original price: Strike-through if discounted
  - Discount badge: '-20%' top-right corner, red/orange background
  - Product name: 14-16sp/pt, 2 lines max with ellipsis
  - Rating: 5 stars + review count '(234)'
  - Wishlist heart: Top-right, outline or filled
  - Quick add: Cart icon button",

            ScreenType.ProductDetail => @"
- Image gallery: Full-width carousel, swipeable, dot indicators, 1:1 or 4:3 ratio
- Product title: 24sp/pt bold, 16px margins
- Price: 28sp/pt bold, primary color
- Original price: Strike-through, 18sp/pt, gray
- Rating: Star display + review count '4.5 (234 reviews)', tappable
- Color selector: Color swatch circles, selected state with checkmark or ring
- Size selector: Pill buttons (S/M/L/XL), selected state with background fill
- Quantity selector: - [1] + stepper buttons, 40px height
- Add to Cart button: 56px height, full-width or prominent, sticky bottom or after description
- Product description: 16sp/pt, expandable with 'Read more' link for long text
- Specifications: List format, label + value pairs
- Reviews section: Summary stats + top 2-3 reviews with 'See all' link
- Similar products: Horizontal scroll of product cards",

            ScreenType.Cart => @"
- Header: 'Cart' title with item count '(3 items)'
- Cart item cards: Repeating list, 100-120px height each
  - Product thumbnail: 80x80px, rounded corners
  - Product name: 16sp/pt, 2 lines max
  - Size/color info: 12sp/pt gray below name
  - Price: 16sp/pt bold, right-aligned
  - Quantity stepper: - [N] + buttons
  - Remove button: X icon or 'Remove' link
- Item subtotal: Below each item or omitted
- Promo code section: Expandable, input field + 'Apply' button
- Summary card: Subtotal, Shipping, Tax, Total (bold, large)
- Checkout button: 56px height, full-width, 'Proceed to Checkout' + total on button
- Continue shopping: Link above or below checkout button
- Empty state: Illustration + 'Your cart is empty' + 'Start Shopping' button",

            ScreenType.Checkout => @"
- Progress indicator: Step breadcrumb (1.Cart → 2.Shipping → 3.Payment → 4.Review)
- Section headers: 'Shipping Address', 'Payment Method' (18sp/pt bold)
- Form fields: 56px height, proper labels and icons
- Saved address selector: Radio buttons or cards with selected state
- Shipping method: Radio options (Standard $5.99, Express $12.99)
- Payment method: Credit card form or saved card selector with card brand logos
- Card input: Number (16 digits), Expiry (MM/YY), CVV (3 digits)
- Billing checkbox: 'Same as shipping address'
- Order summary: Collapsible section, shows items + totals
- Promo code: Input with 'Apply' button
- Place order button: 56px height, 'Place Order - $XX.XX' with total
- Security badges: SSL lock icon, 'Secure checkout', payment logos
- Terms: Checkbox + 'I agree to Terms and Conditions' with link",

            ScreenType.Orders => @"
- Tab navigation: 'Active', 'Delivered', 'Cancelled' (switch between order states)
- Order cards: Repeating list, 120-140px height
  - Order number: '#12345' bold
  - Order date: '15 Jan 2024' gray
  - Status badge: Pill-shaped, color-coded (yellow=processing, green=delivered, gray=cancelled)
  - Product thumbnails: 2-3 small images (48x48px) + '+2 more' if additional items
  - Total amount: Bold, right-aligned
  - Track button: For shipped orders, 'Track Package'
  - Reorder button: Quick reorder functionality
- Tap behavior: Expand card or navigate to order details
- Empty state: Per tab, 'No active orders' with illustration
- Filter/sort: Optional dropdown for date range, status",

            ScreenType.PatientsList => @"
- Search bar: 48px height, search icon, 'Search patients' placeholder, prominent top position
- Filter chips: Horizontal scroll, 'All', 'Critical', 'Monitoring', 'Stable' with counts
- Sort dropdown: 'Sort by: Name' or 'Recent' or 'Status'
- Patient cards: Repeating list, 88-104px height per card
  - Patient photo: 48-64px circle, left-aligned, professional headshot
  - Name: 18sp/pt bold, primary text color
  - Age/Gender: 14sp/pt gray text below name ('45, Male' or '32, F')
  - Status indicator: 8px circle dot before or beside name (green=stable, yellow=attention, red=critical)
  - Condition tags: Pill-shaped chips below patient info (Diabetes, Hypertension, etc.)
  - Last visit: 12sp/pt gray, right side ('2 days ago')
  - Chevron: Right arrow (>) indicating drill-down capability
- Floating Action Button (FAB): + icon, 56x56px, bottom-right, 'Add new patient'
- Dividers: 1px between cards OR no divider with card backgrounds
- Pull to refresh: Standard pull-down gesture
- Empty state: Illustration + 'No patients found' + search/filter suggestions",

            ScreenType.PatientDetail => @"
- Header bar: Patient name (20-24sp/pt bold), back button, status badge (Critical/Stable)
- Patient photo: 96px circle, top section, edit button overlay (camera icon)
- Info card row: Age, Blood type, Known allergies with icons (24x24)
- Vitals section: Current vital signs in grid (Heart rate, BP, Temperature, SpO2)
  - Large numbers (24-28sp/pt) with units
  - Status indicators (color-coded: normal/warning/critical)
- Tab navigation: History, Medications, Appointments, Documents (switch content)
- Medical timeline (History tab): Vertical timeline with date markers
  - Event cards: Date, doctor name, diagnosis, visit notes
  - Expandable details: Tap to show full information
- Medications list: Drug name, dosage, frequency, prescribing doctor
- Appointments list: Upcoming and past with date/time/type
- Action buttons: 'Schedule Appointment', 'Add Note', 'Call Patient' (48px height)
- Emergency contact: Quick access card with phone icon and call action",

            ScreenType.Appointments => @"
- Calendar header: Month/Week view toggle, current date highlighted
- Date selector: Horizontal scrollable date picker with today centered
- Upcoming section: 'Upcoming Appointments' header with count
- Appointment cards: Repeating list, 104-120px height
  - Date/Time: 24sp/pt bold, top-left ('Mon, Jan 15 • 2:30 PM')
  - Duration: 14sp/pt below time ('30 minutes')
  - Patient info: Name + 40px photo inline, or doctor name for patient-facing
  - Appointment type: Icon indicating video call or in-person visit
  - Status badge: 'Upcoming' (blue), 'Completed' (green), 'Cancelled' (gray)
  - Action buttons: 'Join Call' (for telemedicine), 'Reschedule', 'Cancel'
- Floating Action Button: + icon, 'Schedule new appointment'
- Filter options: 'All', 'Today', 'This Week', 'Telemedicine Only'
- Empty state: 'No appointments scheduled' with calendar illustration
- Sync indicator: 'Synced with Google Calendar' with icon, subtle bottom text",

            ScreenType.CalendarSync => @"
- Header: 'Calendar Integration' title (24sp/pt), back button
- Status card: Large card showing sync status
  - Google Calendar icon (48x48px) or Microsoft/Apple logos
  - Connection status: 'Connected' (green) or 'Not Connected' (gray)
  - Connected account: Email address with small profile photo
  - Disconnect button: 'Disconnect' secondary button if connected
- Sync options section: Toggle switches, right-aligned
  - Auto-sync appointments: On/Off toggle
  - Two-way sync: On/Off toggle with info icon (explain bidirectional)
  - Sync appointment reminders: On/Off
  - Include patient names in calendar: On/Off with privacy warning icon
- Sync frequency: Dropdown selector ('Real-time', 'Every 15 minutes', 'Hourly', 'Daily')
- Calendar selection: 'Select which calendar' dropdown (Main, Work, Personal)
- Last synced: Text with timestamp 'Last synced: 2 minutes ago' + refresh icon button
- Privacy notice card: HIPAA compliance warning with info icon
  - 'Patient data is encrypted and HIPAA compliant'
- Connect button: If not connected, large Google-branded 'Connect Google Calendar' button (56px)
- Sync now button: 'Sync Now' button to manually trigger sync
- Advanced settings: Collapsible section for conflict resolution, sync direction",

            ScreenType.Feed => @"
- Pull-to-refresh: Top overscroll gesture
- Stories (optional): Horizontal scroll at top, 64px circular avatars with gradient rings
- Post cards: Repeating, full-width or with 16px side margins
  - User avatar: 40px circle, top-left
  - Username: 16sp/pt bold beside avatar
  - Timestamp: 12sp/pt gray, right-aligned ('2h ago')
  - Post content: Text (16sp/pt) + images/videos full-width within card
  - Image gallery: Swipeable if multiple, aspect ratio varies
  - Actions bar: Like (heart), Comment (bubble), Share (arrow) icons with counts
  - Like count: '234 likes' (14sp/pt)
  - View comments: 'View all 45 comments' link, shows preview of top 1-2
- Spacing: 16-24px between post cards
- Load more: Infinite scroll OR 'Load More' button
- Filter tabs (optional): 'Following', 'For You', 'Trending' at top",

            ScreenType.Detail => @"
- Top bar: Back button (< left), Share icon, Bookmark icon, More menu (three dots)
- Hero media: Full-width image or video, 16:9 or square aspect ratio
- Title: 28-32sp/pt bold, 16-24px side margins
- Metadata row: Author name, date published, category chip/tag
- Author info: Small avatar (32px) + name + follow button (optional)
- Body content: 16sp/pt, 1.5 line height, max-width 600px for readability
  - Paragraphs with spacing
  - Inline images/videos with captions
  - Pull quotes or callouts
- Tags: Pill-shaped tag chips
- Actions: Share, Save, Like buttons (sticky or inline)
- Related section: 'You might also like' with 3-4 cards in horizontal scroll
- Comments: Expandable section, 'Show comments (45)' with preview",

            ScreenType.Search => @"
- Search bar: 56px height, auto-focus on screen load, clear X button when typing
- Voice search: Microphone icon right side of search bar (24x24px)
- Search icon: Left side of input field (24x24px)
- Recent searches: Section header 'Recent', list with clock icon (20x20), X to remove individual
- Clear all: 'Clear All' link aligned right beside 'Recent' header
- Search suggestions: As-you-type dropdown, matching text highlighted
- Filter chips: Horizontal scroll below search bar (Category, Date, Price)
- Results header: 'Results for ""[query]""' with result count
- Results layout: Grid or List based on content type (products, articles, people)
- No results: 'No results for ""[query]""' with illustration
  - Suggestions: 'Try searching for...' with example queries
- Cancel button: Top-right, returns to previous screen",

            _ => "Standard screen layout with appropriate header, content area, and navigation components."
        };
    }

    /// <summary>
    /// Get category-specific component additions
    /// </summary>
    private string GetCategorySpecificComponents(ScreenType screenType, string category)
    {
        if (category.Equals("healthcare", StringComparison.OrdinalIgnoreCase))
        {
            return screenType switch
            {
                ScreenType.Login => "- HIPAA compliance badge: 'HIPAA Secure' with lock icon",
                ScreenType.Dashboard => "- Patient alerts: Color-coded urgent notifications\n- Appointment summary: Today's schedule preview\n- Sync status: Google Calendar sync indicator",
                ScreenType.Profile => "- Medical license number\n- Specialty/Department\n- Hospital affiliation",
                _ => string.Empty
            };
        }

        if (category.Equals("ecommerce", StringComparison.OrdinalIgnoreCase))
        {
            return screenType switch
            {
                ScreenType.Home => "- Category quick links: Icon grid for shopping categories\n- Flash sale banner: Time-limited offers\n- Recently viewed: Horizontal scroll of products",
                ScreenType.Dashboard => "- Order tracking: Current orders with progress\n- Wishlist preview: Saved items count\n- Rewards/points: Loyalty program balance",
                _ => string.Empty
            };
        }

        if (category.Equals("finance", StringComparison.OrdinalIgnoreCase))
        {
            return screenType switch
            {
                ScreenType.Login => "- Biometric login: Face ID / Fingerprint option\n- Security indicators: Encryption badges",
                ScreenType.Dashboard => "- Account balance: Large, with hide/show toggle\n- Quick actions: Pay, Transfer, Deposit buttons\n- Spending chart: Category breakdown",
                _ => string.Empty
            };
        }

        return string.Empty;
    }

    /// <summary>
    /// Get navigation pattern appropriate for category
    /// </summary>
    public string GetNavigationPattern(string category, ScreenType screenType)
    {
        var categoryPattern = category.ToLowerInvariant() switch
        {
            "healthcare" => UIDesignKnowledgeBase.NavigationPatterns.BottomTabNavigation + "\n" +
                          UIDesignKnowledgeBase.GetCategoryTabs("healthcare"),
            "ecommerce" => UIDesignKnowledgeBase.NavigationPatterns.HybridNavigation + "\n" +
                         UIDesignKnowledgeBase.GetCategoryTabs("ecommerce"),
            "finance" => UIDesignKnowledgeBase.NavigationPatterns.BottomTabNavigation + "\n" +
                       UIDesignKnowledgeBase.GetCategoryTabs("finance"),
            "social" => UIDesignKnowledgeBase.NavigationPatterns.TopNavigation + "\n" +
                      UIDesignKnowledgeBase.GetCategoryTabs("social"),
            "fitness" => UIDesignKnowledgeBase.NavigationPatterns.BottomTabNavigation + "\n" +
                       UIDesignKnowledgeBase.GetCategoryTabs("fitness"),
            "education" => UIDesignKnowledgeBase.NavigationPatterns.BottomTabNavigation + "\n" +
                         UIDesignKnowledgeBase.GetCategoryTabs("education"),
            "productivity" => UIDesignKnowledgeBase.NavigationPatterns.BottomTabNavigation + "\n" +
                            UIDesignKnowledgeBase.GetCategoryTabs("productivity"),
            _ => UIDesignKnowledgeBase.NavigationPatterns.BottomTabNavigation
        };

        // Some screens don't need full navigation
        if (screenType is ScreenType.Login or ScreenType.Signup or ScreenType.Onboarding)
        {
            return "Minimal navigation - back button only if part of flow, no bottom tabs.";
        }

        return categoryPattern;
    }

    /// <summary>
    /// Get layout pattern for screen type
    /// </summary>
    public string GetLayoutPattern(ScreenType screenType)
    {
        return screenType switch
        {
            ScreenType.ProductList => UIDesignKnowledgeBase.LayoutPatterns.GridLayout,
            ScreenType.PatientsList => UIDesignKnowledgeBase.LayoutPatterns.ListLayout,
            ScreenType.Orders => UIDesignKnowledgeBase.LayoutPatterns.ListLayout,
            ScreenType.Dashboard => UIDesignKnowledgeBase.LayoutPatterns.CardLayout,
            ScreenType.Home => UIDesignKnowledgeBase.LayoutPatterns.CardLayout,
            ScreenType.Feed => UIDesignKnowledgeBase.LayoutPatterns.CardLayout,
            ScreenType.Settings => UIDesignKnowledgeBase.LayoutPatterns.ListLayout,
            ScreenType.Profile => UIDesignKnowledgeBase.LayoutPatterns.SingleColumn,
            ScreenType.ProductDetail => UIDesignKnowledgeBase.LayoutPatterns.SingleColumn,
            ScreenType.PatientDetail => UIDesignKnowledgeBase.LayoutPatterns.SingleColumn,
            _ => UIDesignKnowledgeBase.LayoutPatterns.SingleColumn
        };
    }

    /// <summary>
    /// Get 8px spacing guidance for platform
    /// </summary>
    public string GetSpacingGuidance(string platform)
    {
        var unit = platform.Equals("iOS", StringComparison.OrdinalIgnoreCase) ? "points" : "dp";
        return $@"Use 8{unit} grid system:
- XS: 8{unit} (tight spacing, small gaps)
- SM: 16{unit} (default spacing, card padding)
- MD: 24{unit} (section spacing)
- LG: 32{unit} (large gaps, component separation)
- XL: 48{unit} (major sections)
- XXL: 64{unit} (page sections)

{UIDesignKnowledgeBase.SpacingSystem.GridSystem}";
    }

    /// <summary>
    /// Get state variation for a screen
    /// </summary>
    public string GetStateVariation(ScreenType screenType, string state)
    {
        var baseComponents = GetBaseScreenComponents(screenType);
        var stateOverlay = state.ToLowerInvariant() switch
        {
            "empty" => UIDesignKnowledgeBase.StateVariations.EmptyState,
            "loading" => UIDesignKnowledgeBase.StateVariations.LoadingState,
            "error" => UIDesignKnowledgeBase.StateVariations.ErrorState,
            "success" => UIDesignKnowledgeBase.StateVariations.SuccessState,
            _ => string.Empty
        };

        return $@"{baseComponents}

STATE VARIATION: {state.ToUpperInvariant()}
{stateOverlay}

Show the screen in the {state} state, replacing the main content area with the state UI while keeping the navigation/header structure.";
    }

    /// <summary>
    /// Build prompts for multi-screen flow with continuity
    /// </summary>
    public List<(ScreenType, string)> BuildMultiScreenFlow(List<ScreenType> screens, string category, string appName, string platform)
    {
        var result = new List<(ScreenType, string)>();

        for (int i = 0; i < screens.Count; i++)
        {
            var screenType = screens[i];
            var flowContext = $"This is screen {i + 1} of {screens.Count} in the {appName} app flow.";

            if (i > 0)
            {
                flowContext += $" Previous screen: {screens[i - 1]}.";
            }

            if (i < screens.Count - 1)
            {
                flowContext += $" Next screen: {screens[i + 1]}.";
            }

            flowContext += " Maintain visual continuity with consistent colors, typography, and component styling across all screens.";

            var components = GetUIComponentsForScreen(screenType, category);
            var navigation = GetNavigationPattern(category, screenType);
            var layout = GetLayoutPattern(screenType);

            var prompt = $@"{flowContext}

Create a {platform} {screenType} screen for ""{appName}"", a {category} app.

{components}

NAVIGATION: {navigation}
LAYOUT: {layout}

Ensure this screen visually matches the overall app design system.";

            result.Add((screenType, prompt));
        }

        return result;
    }

    /// <summary>
    /// Build UI variation prompts for A/B testing
    /// </summary>
    public List<string> BuildUIVariationPrompts(AppResourcesGenerationRequest request, int count = 3)
    {
        var screenType = request.Options.ScreenTypes.FirstOrDefault();
        var category = request.Options.AppCategory ?? "App";
        var appName = request.Options.AppName ?? "MyApp";
        var platform = request.Options.TargetPlatform?.ToString() ?? request.Platforms.FirstOrDefault() ?? "iOS";

        var baseComponents = GetUIComponentsForScreen(screenType, category);

        var variations = new List<string>
        {
            // Variation 1: Minimalist
            $@"Create a MINIMALIST {platform} {screenType} screen for ""{appName}"".

Style: Clean, minimal, maximum white space
- Thin line weights (1-2px)
- Limited color palette (2-3 colors)
- Generous spacing (32-48px between sections)
- Simple geometric shapes
- Sans-serif typography only
- Subtle or no shadows

{baseComponents}",

            // Variation 2: Visual-first
            $@"Create a VISUAL-FIRST {platform} {screenType} screen for ""{appName}"".

Style: Bold, image-heavy, engaging
- Large hero images or illustrations
- Vibrant gradient backgrounds
- Bold, expressive typography
- Rich colors and depth
- Prominent visual elements
- Card-based with elevation

{baseComponents}",

            // Variation 3: Data-dense
            $@"Create a DATA-DENSE {platform} {screenType} screen for ""{appName}"".

Style: Compact, information-rich, efficient
- Compact spacing (8-16px)
- Multiple data points visible
- Small but readable text (14-16sp/pt)
- Tables or dense lists
- Charts and metrics prominent
- Efficient use of space

{baseComponents}"
        };

        return variations.Take(count).ToList();
    }

    /// <summary>
    /// Sanitize UI prompt to avoid content policy issues
    /// </summary>
    public string SanitizeUIPrompt(string prompt)
    {
        if (string.IsNullOrEmpty(prompt))
            return string.Empty;

        // Remove emphasis markers
        prompt = prompt.Replace("**", "").Replace("__", "");

        // Soften imperative language that might trigger content policy
        prompt = prompt.Replace("MUST", "should")
                      .Replace("NEVER", "avoid")
                      .Replace("ALWAYS", "preferably")
                      .Replace("REQUIRED", "recommended")
                      .Replace("CRITICAL", "important");

        // Remove excessive exclamation marks
        prompt = System.Text.RegularExpressions.Regex.Replace(prompt, "!+", ".");

        return prompt.Trim();
    }

    /// <summary>
    /// Analyze UI prompt quality
    /// </summary>
    public UIPromptQualityScore AnalyzeUIPromptQuality(string prompt)
    {
        if (string.IsNullOrEmpty(prompt))
        {
            return new UIPromptQualityScore { OverallScore = 0 };
        }

        var score = new UIPromptQualityScore
        {
            HasComponentSpecification = prompt.Contains("px") || prompt.Contains("dp") || prompt.Contains("pt") ||
                                       prompt.Contains("button") || prompt.Contains("card") || prompt.Contains("field"),
            HasLayoutGuidance = prompt.Contains("layout") || prompt.Contains("grid") || prompt.Contains("column") ||
                               prompt.Contains("spacing") || prompt.Contains("margin"),
            HasSpacingSystem = prompt.Contains("8px") || prompt.Contains("8dp") || prompt.Contains("8pt") ||
                              prompt.Contains("grid system") || prompt.Contains("16px") || prompt.Contains("24px"),
            HasAccessibilityConsiderations = prompt.Contains("accessibility") || prompt.Contains("44x44") ||
                                            prompt.Contains("48x48") || prompt.Contains("contrast") ||
                                            prompt.Contains("WCAG") || prompt.Contains("touch target"),
            HasPlatformGuidelines = prompt.Contains("iOS") || prompt.Contains("Android") || prompt.Contains("Material") ||
                                   prompt.Contains("platform") || prompt.Contains("HIG") || prompt.Contains("Fluent"),
            HasNavigationPattern = prompt.Contains("navigation") || prompt.Contains("tab") || prompt.Contains("drawer") ||
                                  prompt.Contains("app bar") || prompt.Contains("bottom bar")
        };

        // Calculate overall score
        int criteriaCount = 0;
        int metCriteria = 0;

        criteriaCount++; if (score.HasComponentSpecification) metCriteria++;
        criteriaCount++; if (score.HasLayoutGuidance) metCriteria++;
        criteriaCount++; if (score.HasSpacingSystem) metCriteria++;
        criteriaCount++; if (score.HasAccessibilityConsiderations) metCriteria++;
        criteriaCount++; if (score.HasPlatformGuidelines) metCriteria++;
        criteriaCount++; if (score.HasNavigationPattern) metCriteria++;

        score.OverallScore = criteriaCount > 0 ? (metCriteria * 100.0 / criteriaCount) : 0;

        return score;
    }

    /// <summary>
    /// Get component library for platform
    /// </summary>
    private string GetComponentLibrary(string platform)
    {
        return platform.ToLowerInvariant() switch
        {
            "ios" => UIDesignKnowledgeBase.iOSHumanInterfaceGuidelines,
            "android" => UIDesignKnowledgeBase.MaterialDesign3,
            "web" => UIDesignKnowledgeBase.MaterialDesign3,
            "macos" => UIDesignKnowledgeBase.iOSHumanInterfaceGuidelines,
            _ => UIDesignKnowledgeBase.MaterialDesign3
        };
    }

    /// <summary>
    /// Get industry pattern for category
    /// </summary>
    private string GetIndustryPattern(string category)
    {
        return category.ToLowerInvariant() switch
        {
            "healthcare" => UIDesignKnowledgeBase.IndustryPatterns.Healthcare,
            "ecommerce" => UIDesignKnowledgeBase.IndustryPatterns.Ecommerce,
            "finance" => UIDesignKnowledgeBase.IndustryPatterns.Finance,
            "social" => UIDesignKnowledgeBase.IndustryPatterns.SocialMedia,
            "fitness" => UIDesignKnowledgeBase.IndustryPatterns.Fitness,
            "education" => UIDesignKnowledgeBase.IndustryPatterns.Education,
            "productivity" => UIDesignKnowledgeBase.IndustryPatterns.Productivity,
            _ => "Standard app UI patterns with clean, modern design."
        };
    }
}

/// <summary>
/// Quality score for UI prompt analysis
/// </summary>
public class UIPromptQualityScore
{
    public bool HasComponentSpecification { get; set; }
    public bool HasLayoutGuidance { get; set; }
    public bool HasSpacingSystem { get; set; }
    public bool HasAccessibilityConsiderations { get; set; }
    public bool HasPlatformGuidelines { get; set; }
    public bool HasNavigationPattern { get; set; }
    public double OverallScore { get; set; }
}
