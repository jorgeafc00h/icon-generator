namespace IconGenerator.Functions.Prompts;

/// <summary>
/// UI/UX design knowledge base for professional screen mockup generation.
/// Contains Material Design 3, iOS HIG, industry patterns, and accessibility guidelines.
/// </summary>
public static class UIDesignKnowledgeBase
{
    /// <summary>
    /// Material Design 3 component specifications
    /// </summary>
    public const string MaterialDesign3 = @"
MATERIAL DESIGN 3 COMPONENT LIBRARY:

BUTTONS:
- Height: 40dp (standard), 48dp (large)
- Padding: 16dp horizontal, 8dp vertical
- Corner radius: 20dp (fully rounded)
- Touch target: 48x48dp minimum
- Types: Filled, Outlined, Text, Elevated, Tonal

CARDS:
- Corner radius: 12dp
- Padding: 16dp
- Elevation: 1dp (outlined), 3dp (elevated)
- Content spacing: 16dp between elements

TEXT FIELDS:
- Height: 56dp (outlined), 48dp (filled)
- Corner radius: 4dp (top) for filled
- Label: 12sp when focused/filled
- Helper text: 12sp below field

TYPOGRAPHY SCALE:
- Display Large: 57sp
- Display Medium: 45sp
- Display Small: 36sp
- Headline Large: 32sp
- Headline Medium: 28sp
- Headline Small: 24sp
- Title Large: 22sp
- Title Medium: 16sp
- Title Small: 14sp
- Body Large: 16sp
- Body Medium: 14sp
- Body Small: 12sp
- Label Large: 14sp
- Label Medium: 12sp
- Label Small: 11sp

COLOR SYSTEM:
- Primary, Secondary, Tertiary roles
- On-Primary, On-Secondary (text colors)
- Surface, Background, Outline
- Error, Success (semantic colors)
- Container variants for each role

SPACING:
- Base unit: 4dp
- Common: 8dp, 12dp, 16dp, 24dp, 32dp, 48dp, 64dp
";

    /// <summary>
    /// iOS Human Interface Guidelines
    /// </summary>
    public const string iOSHumanInterfaceGuidelines = @"
iOS HUMAN INTERFACE GUIDELINES:

BUTTONS:
- Minimum touch target: 44x44 points
- System button height: 44pt
- Corner radius: 8-12pt (varies by style)
- Padding: 16pt horizontal

NAVIGATION:
- Navigation bar: 44pt (compact), 96pt (large title)
- Tab bar: 49pt height
- Search bar: 44pt height
- Status bar: 44pt (with notch), 20pt (without)

TYPOGRAPHY (SF Pro):
- Large Title: 34pt
- Title 1: 28pt
- Title 2: 22pt
- Title 3: 20pt
- Headline: 17pt (semibold)
- Body: 17pt (regular)
- Callout: 16pt
- Subheadline: 15pt
- Footnote: 13pt
- Caption 1: 12pt
- Caption 2: 11pt

COLORS:
- System colors: Blue, Green, Red, Orange, Yellow, Pink, Purple
- Dynamic colors adapt to light/dark mode
- Vibrancy effects for layering

SPACING:
- Base unit: 8pt
- Edge margins: 16pt
- Section spacing: 24-32pt
- List item height: 44pt minimum

CARDS & LISTS:
- Corner radius: 10-12pt
- List separators: 1pt hairline
- Grouped lists: inset 16pt
";

    /// <summary>
    /// Microsoft Fluent Design System
    /// </summary>
    public const string FluentDesign = @"
MICROSOFT FLUENT DESIGN SYSTEM:

BUTTONS:
- Height: 32px (standard), 40px (large)
- Padding: 16px horizontal
- Corner radius: 4px (standard), 2px (subtle)

ELEVATION:
- Level 4: Dialog shadows
- Level 8: Flyout shadows
- Level 16: Popup shadows
- Level 64: Teaching tip shadows

ACRYLIC MATERIAL:
- Background blur with transparency
- Noise texture overlay
- Tint color at 60-80% opacity

TYPOGRAPHY:
- Header: 28px
- Subheader: 20px
- Title: 16px (semibold)
- Body: 14px
- Caption: 12px

SPACING:
- Base unit: 4px
- Common: 8px, 12px, 16px, 20px, 24px, 32px
";

    /// <summary>
    /// Navigation pattern specifications
    /// </summary>
    public static class NavigationPatterns
    {
        public const string BottomTabNavigation = @"
BOTTOM TAB NAVIGATION:
- Height: 80dp Android, 49pt iOS
- Active indicator: 64dp wide Android
- Items: 4-5 tabs maximum for usability
- Icon size: 24x24dp
- Label: 12sp (Material), 10pt (iOS)
- Spacing: Equal distribution across width
- Active state: Icon + label in primary color
- Inactive state: Icon + label in neutral gray
- Position: Fixed at bottom, always visible
";

        public const string TopNavigation = @"
TOP NAVIGATION (APP BAR):
- Height: 56dp Android (phone), 64dp (tablet), 44-96pt iOS
- Title: 20sp (Material), 17pt bold (iOS)
- Actions: Right-aligned, 48x48dp touch targets
- Back button: Left-aligned, 48x48dp
- Shadow/elevation: 4dp Material, none iOS (uses border)
- Search: Can expand full-width or inline
";

        public const string DrawerNavigation = @"
DRAWER NAVIGATION (SIDE MENU):
- Width: 360dp maximum (Material), 75% screen iOS
- Modes: Overlay (mobile), Permanent (desktop)
- Header: 164dp height with account info
- Items: 48dp height, 16dp padding, icons 24x24dp
- Sections: Dividers between groups
- Selected state: Background tint + bold text
";

        public const string HybridNavigation = @"
HYBRID NAVIGATION:
- Top app bar: Primary actions, title, search
- Bottom tabs: Main sections (3-5 items)
- Drawer: Settings, account, secondary navigation
- Use for complex apps with multiple sections
";
    }

    /// <summary>
    /// Layout pattern specifications
    /// </summary>
    public static class LayoutPatterns
    {
        public const string SingleColumn = @"
SINGLE COLUMN LAYOUT:
- Mobile-first design
- Full-width content blocks
- Side margins: 16px mobile, 24px tablet
- Max width: 600px for readability
- Vertical rhythm: 24px between sections
- Best for: Reading, forms, simple flows
";

        public const string TwoColumn = @"
TWO COLUMN LAYOUT:
- Tablet/desktop breakpoint: 768px+
- Ratio: 8/4 (sidebar) or 6/6 (equal)
- Gutter: 24px between columns
- Responsive: Stacks to single column on mobile
- Best for: Content + sidebar, settings, dashboards
";

        public const string GridLayout = @"
GRID LAYOUT:
- Columns: 2 (mobile), 3-4 (tablet/desktop)
- Gutters: 16px mobile, 24px desktop
- Card aspect ratio: 1:1 (square) or 4:3
- Equal heights within row
- Best for: Products, galleries, portfolios
";

        public const string ListLayout = @"
LIST LAYOUT:
- Item height: 56-72dp (one line), 88dp (two lines)
- Dividers: 1px between items or no divider
- Swipe actions: Left (delete), Right (archive/flag)
- Avatar/icon: 40-48px circle, left-aligned
- Chevron: Right-aligned for drill-down
- Best for: Messages, contacts, transactions
";

        public const string CardLayout = @"
CARD LAYOUT:
- Card spacing: 16-24px between cards
- Corner radius: 12px
- Padding: 16px internal
- Shadow: 2-4dp elevation
- Content: Image header + text + actions
- Best for: News feeds, dashboards, mixed content
";
    }

    /// <summary>
    /// 8px spacing system - industry standard
    /// </summary>
    public static class SpacingSystem
    {
        public const string GridSystem = @"
8PX GRID SYSTEM (INDUSTRY STANDARD):

WHY 8PX:
- Scales perfectly: @1x, @2x, @3x displays
- Used by Google, Apple, Microsoft, Airbnb
- Reduces decision fatigue
- Creates consistent rhythm

SCALE:
- XXS: 4px (hairlines, fine details)
- XS: 8px (tight spacing, small gaps)
- SM: 16px (default spacing, card padding)
- MD: 24px (section spacing)
- LG: 32px (large gaps, component separation)
- XL: 48px (major sections)
- XXL: 64px (page sections, hero spacing)

TOUCH TARGETS:
- iOS: 44x44px minimum (HIG requirement)
- Android: 48x48dp minimum (Material guideline)
- Web: 44px minimum for accessibility

COMPONENT SIZING:
- Button height: 40-48px (5-6 grid units)
- Input fields: 40-56px
- List items: 56-72px (7-9 units)
- Card padding: 16px (2 units)
- Screen margins: 16-24px

APPLY TO:
- Margins, padding, gaps
- Component dimensions
- Line heights (multiples of 4-8px)
- Icon sizes: 16, 24, 32, 48px
";
    }

    /// <summary>
    /// Industry-specific UI patterns
    /// </summary>
    public static class IndustryPatterns
    {
        public const string Healthcare = @"
HEALTHCARE UI PATTERNS:

PATIENT CARDS:
- Photo: 48-64px circle, professional headshot
- Name: 18sp bold, prominent
- Age/Gender: 14sp gray below name
- Status indicator: Color dot (green=healthy, yellow=attention, red=critical)
- Condition tags: Pill-shaped chips (diabetes, hypertension)
- Chevron: Right arrow for drill-down
- Height: 88-104px

APPOINTMENT CARDS:
- Date/Time: Large, 24sp bold at top
- Duration: 14sp below time
- Patient info: Name + photo inline
- Type: Video call icon or in-person icon
- Status: Upcoming, Completed, Cancelled
- Actions: Reschedule, Cancel buttons

MEDICAL TIMELINE:
- Vertical timeline with dots
- Date stamps on left
- Event cards on right
- Color coding by event type (visits, tests, medications)
- Expandable details

TRUST INDICATORS:
- HIPAA compliance badge
- Lock icons for security
- Doctor credentials (MD, specialty)
- Hospital affiliations
- Verification checkmarks

COLORS:
- Primary: Blues (#0066CC trust), greens (#00A86B health)
- Avoid: Heavy reds (anxiety)
- Status: Green (stable), Yellow (monitor), Red (urgent)
- Background: Clean whites, light grays

NAVIGATION:
- Bottom tabs: Dashboard, Patients, Appointments, Messages, Profile
- Quick actions: FAB for add patient/appointment
";

        public const string Ecommerce = @"
ECOMMERCE UI PATTERNS:

PRODUCT CARDS:
- Image: Square or 4:3 aspect ratio, high quality
- Price: Large, bold (24-28sp), prominent color
- Original price: Strike-through if on sale
- Discount badge: Top-right corner, red/orange
- Rating: 5-star display + review count
- Heart icon: Wishlist, top-right
- Quick add: Cart icon button

PRODUCT GRID:
- 2 columns mobile, 3-4 desktop
- 16px gutters
- Equal card heights
- Lazy loading images
- Infinite scroll or pagination

CART UI:
- Item thumbnail: 80x80px
- Quantity stepper: - [2] + buttons
- Remove icon: X or trash can
- Subtotal: Per item and total
- Sticky footer: Total + Checkout button
- Continue shopping link

CHECKOUT FLOW:
- Progress indicator: 1. Cart → 2. Shipping → 3. Payment → 4. Review
- Saved addresses: Radio select
- Payment methods: Cards with logos (Visa, Mastercard)
- Security badges: SSL, verified checkout
- Order summary: Sticky sidebar

COLORS:
- CTAs: High contrast (orange, green for buy)
- Sale prices: Red or orange
- Trust: Blue for secure elements
";

        public const string Finance = @"
FINANCE UI PATTERNS:

ACCOUNT BALANCE:
- Large display: 32-48sp
- Hide/show toggle: Eye icon
- Trend indicator: ↑ +2.5% green or ↓ -1.2% red
- Period selector: Day, Week, Month, Year tabs
- Chart: Line or area chart below

TRANSACTION LIST:
- Merchant logo: 40px circle
- Name: 16sp bold
- Category: 14sp gray (Shopping, Food, etc.)
- Amount: 16sp bold, right-aligned
- Date: 12sp gray below merchant
- Income: Green, Expenses: Default/red
- Swipe actions: Categorize, flag

BUDGET CARDS:
- Category name + icon
- Progress bar: Spent vs Budget
- Amount text: $450 / $800
- Color: Green (under), Yellow (near), Red (over)
- Percentage: 56% of budget

SECURITY:
- Lock icons throughout
- Biometric prompts: Face ID, fingerprint
- Two-factor authentication
- Session timeouts
- Encryption badges

COLORS:
- Professional: Blues, grays
- Positive: Greens (gains, income)
- Negative: Reds (losses, overspending)
- Neutral: Dark text on white/light gray
";

        public const string SocialMedia = @"
SOCIAL MEDIA UI PATTERNS:

POSTS:
- Avatar: 40px circle, top-left
- Username: 16sp bold, handle 14sp gray
- Timestamp: 12sp gray, top-right
- Content: Text 16sp, images full-width
- Actions bar: Like (heart), Comment (bubble), Share (arrow)
- Like count: 14sp below post
- View comments: Expandable

STORIES:
- Circle avatars: 64px diameter
- Active ring: 3px gradient border
- Horizontal scroll
- Viewed: Gray ring
- Add story: + icon in circle

NOTIFICATIONS:
- Avatar: 40px
- Action text: '[User] liked your post' with bold username
- Content preview: Thumbnail or text snippet
- Timestamp: '2h ago'
- Unread indicator: Blue dot or background tint
- Swipe to delete

PROFILE:
- Cover photo: 16:9 ratio
- Profile photo: 96-120px circle, overlapping cover
- Stats: Posts, Followers, Following in row
- Bio: 2-3 lines max, centered or left
- Action button: Follow, Message, Edit
- Grid or list toggle
";

        public const string Fitness = @"
FITNESS UI PATTERNS:

ACTIVITY RINGS:
- Concentric circles showing progress
- Colors: Move (red), Exercise (green), Stand (blue)
- Center: Large stat (calories, steps)
- Tap to expand details

WORKOUT CARDS:
- Exercise name + icon
- Sets x Reps or Duration
- Rest timer between sets
- Check mark when complete
- Progress: 2 of 3 sets complete

STATS DASHBOARD:
- Today's summary cards
- Charts: Weekly trends, heart rate
- Personal records: Highlighted achievements
- Streaks: Days in a row badge
";

        public const string Education = @"
EDUCATION UI PATTERNS:

COURSE CARDS:
- Thumbnail: 16:9 course image
- Title: 18sp bold
- Instructor: 14sp with small avatar
- Progress bar: % complete
- Duration: Clock icon + time
- Rating: Stars + reviews

LESSON LIST:
- Checkmark: Completed lessons (green)
- Lock icon: Upcoming lessons
- Current: Highlighted row
- Duration per lesson
- Expandable sections by module

QUIZ UI:
- Progress: Question 3 of 10
- Timer: Countdown if timed
- Question: Large, clear text
- Multiple choice: Radio buttons or cards
- Feedback: Immediate (green/red) or end
";

        public const string Productivity = @"
PRODUCTIVITY UI PATTERNS:

TASK LIST:
- Checkbox: Left-aligned, 24px
- Task text: 16sp, strikes through when done
- Due date: Pill chip, color codes by urgency
- Priority: Flag icon or P1/P2/P3
- Project: Tag or color bar
- Drag handle: For reordering

KANBAN BOARD:
- Columns: To Do, In Progress, Done
- Cards: Draggable between columns
- Card content: Title, assignee avatar, due date
- WIP limits: Max items per column
- Add card: + button in each column

CALENDAR:
- Month/Week/Day views
- Event blocks: Height = duration
- Color coding: By calendar or category
- Time grid: 30-min or 1-hour slots
- All-day events: Top bar
";
    }

    /// <summary>
    /// UI state variations
    /// </summary>
    public static class StateVariations
    {
        public const string EmptyState = @"
EMPTY STATE DESIGN:
- Illustration: 200x200px, centered
- Headline: 20-24sp, 1-2 words (""No messages yet"")
- Description: 14-16sp, 2-3 lines explaining why empty
- CTA button: Primary action (""Get Started"", ""Add Item"")
- Color: Light, friendly illustration (not error red)
- Spacing: 24px between elements
- Position: Vertically centered on screen
";

        public const string LoadingState = @"
LOADING STATE DESIGN:
- Skeleton screens: Gray placeholders matching content shape
- Shimmer effect: Animated gradient sweep left-to-right
- Spinner: 48px diameter, primary color, centered
- Progress bar: For determinate loading (file uploads)
- Avoid: Text like ""Loading..."" (visual only is cleaner)
- Duration: Show after 300ms delay to avoid flashing
";

        public const string ErrorState = @"
ERROR STATE DESIGN:
- Icon: 48-64px error icon (not red X - too harsh)
- Headline: 20sp ""Something went wrong""
- Message: 14sp specific error details
- Action button: ""Try Again"" primary button
- Secondary: ""Contact Support"" text button
- Color: Warning orange or muted red (not aggressive red)
- Tone: Friendly, apologetic, helpful
";

        public const string SuccessState = @"
SUCCESS STATE DESIGN:
- Icon: 64px green checkmark in circle
- Headline: 20-24sp ""Success!""
- Message: 14-16sp confirmation (""Payment processed"")
- Auto-dismiss: After 2-3 seconds or manual close
- Animation: Check mark drawing animation
- Color: Green for success
- Next action: Optional button to continue
";
    }

    /// <summary>
    /// Accessibility guidelines (WCAG)
    /// </summary>
    public const string AccessibilityGuidelines = @"
ACCESSIBILITY GUIDELINES (WCAG 2.1):

TOUCH TARGETS:
- Minimum: 44x44px iOS, 48x48dp Android
- Spacing: 8px between targets
- Exception: Inline text links (but prefer larger)

COLOR CONTRAST:
- WCAG AA: 4.5:1 for normal text, 3:1 for large text (18pt+)
- WCAG AAA: 7:1 for normal text, 4.5:1 for large text
- Tools: Use contrast checker
- Don't rely on color alone: Use icons, text, patterns

FOCUS STATES:
- Visible focus ring: 2-3px outline
- Keyboard navigation: Tab through all interactive elements
- Focus order: Logical top-to-bottom, left-to-right
- Skip links: ""Skip to main content""

TEXT:
- Font size: 16px minimum for body text
- Line height: 1.5 for body text
- Line length: 60-80 characters max
- Resizable: Support 200% zoom without breaking

ARIA LABELS:
- Images: Alt text describing content
- Icons: aria-label for icon-only buttons
- Form inputs: Associated labels
- Landmarks: <nav>, <main>, <aside> regions

SCREEN READERS:
- Semantic HTML: Use correct heading levels
- Live regions: Announce dynamic content
- Error messages: Associated with inputs
- Hidden content: aria-hidden for decorative elements

ANIMATIONS:
- Respect prefers-reduced-motion
- Avoid flashing: Nothing flashes >3 times/second
- Pause/stop controls: For auto-playing content
";

    /// <summary>
    /// Get screen template with detailed component specifications
    /// </summary>
    public static string GetScreenTemplate(Models.ScreenType screenType)
    {
        return screenType switch
        {
            Models.ScreenType.Login => @"
LOGIN SCREEN TEMPLATE:
- App logo: 80x80px, centered, top 20% of screen
- App name: 24sp below logo
- Email field: 56px height, envelope icon left, 'Email' label
- Password field: 56px height, lock icon left, 'Password' label, eye icon right
- Forgot password: 14sp link, right-aligned below password
- Sign In button: 48px height, full-width, primary color, 24px below fields
- Divider: 'or continue with' text with lines, 32px below button
- Social buttons: Google + Apple side-by-side, 48px height, branded colors
- Sign up link: 'Don't have an account? Sign up' centered bottom
- Security badge: Small lock icon + 'Secure login' text, bottom corner
- Spacing: 16px between form elements, 24px between sections
",

            Models.ScreenType.Signup => @"
SIGNUP SCREEN TEMPLATE:
- Header: 'Create Account' 28sp bold
- Full name field: 56px height, person icon
- Email field: 56px height, envelope icon
- Password field: 56px height, lock icon, strength indicator
- Confirm password field: 56px height
- Terms checkbox: 'I agree to Terms & Privacy Policy' with links
- Sign Up button: 48px height, primary color, disabled until valid
- Social signup: Google + Apple buttons below
- Login link: 'Already have an account? Log in' at bottom
",

            Models.ScreenType.Home => @"
HOME SCREEN TEMPLATE:
- Navigation: Bottom tabs (5 items) or top bar with menu
- Header: Greeting 'Good morning, [Name]' 24sp + avatar 40px circle
- Search bar: 48px height, prominent position, search icon left
- Hero section: Featured content, full-width card or carousel
- Section headers: 18sp bold with 'See All' link
- Content cards: Grid or list layout, images + titles + metadata
- FAB: Floating action button bottom-right, 56x56px
- Spacing: 16px margins, 24px between sections
",

            Models.ScreenType.Dashboard => @"
DASHBOARD SCREEN TEMPLATE:
- Header: 64px height, title + notifications icon + avatar
- Greeting: 'Hi, [Name]' 20sp with time-based greeting
- Stat cards: 3-4 cards in row, large number 32sp + label + trend arrow
- Chart section: Line/bar chart with period selector (Day/Week/Month)
- Recent activity: List of 4-5 items with icons, scrollable
- Quick actions: 2x2 grid of action cards with icons
- Bottom navigation: 4-5 tabs, current highlighted
- Refresh: Pull-to-refresh capability
- Spacing: 16px card padding, 16px between cards, 24px between sections
",

            Models.ScreenType.Profile => @"
PROFILE SCREEN TEMPLATE:
- Cover photo: Optional 16:9 header image
- Avatar: 96-120px circle, centered or left, edit button overlay
- Name: 24sp bold below/beside avatar
- Bio/subtitle: 14sp gray, 2-3 lines
- Stats row: Followers, Following, Posts in equal columns
- Action button: Edit Profile or Follow, 40px height
- Info cards: Email, Phone, Location with icons
- Settings list: Account, Privacy, Notifications with chevrons
- Sign out: Red text button at bottom
- Spacing: 24px between sections
",

            Models.ScreenType.Settings => @"
SETTINGS SCREEN TEMPLATE:
- Header: 'Settings' title, back button left
- Search: Optional search bar for long settings lists
- Sections: Grouped with headers (Account, Privacy, Notifications)
- List items: 56-64px height, icon 24px left, label, chevron right
- Toggle switches: Right-aligned for boolean settings
- Dividers: Between sections or items
- Destructive actions: 'Delete Account' in red at bottom
- Version info: App version, small gray text, very bottom
- Spacing: 8px within groups, 24px between sections
",

            Models.ScreenType.Onboarding => @"
ONBOARDING SCREEN TEMPLATE:
- Illustration: Large, 60% of screen height, top centered
- Headline: 28sp bold, 1-2 words describing benefit
- Description: 16sp, 2-3 lines explaining feature
- Page indicators: Dots showing progress (screen 1 of 3)
- Next button: 48px height, primary color, bottom right
- Skip button: 'Skip' text button, top right
- Get Started: Replace Next on final screen
- Swipe gesture: Horizontal swipe between screens
- Spacing: 32px between illustration and text
",

            Models.ScreenType.ProductList => @"
PRODUCT LIST SCREEN:
- Search bar: 48px height, top, filters icon right
- Filter chips: Horizontal scroll, category tags
- Sort dropdown: Price, Rating, Newest
- Product grid: 2 columns mobile, 3-4 desktop
- Product cards: Image (square), title, price, rating
- Wishlist icon: Heart, top-right of card
- Quick add: Cart button on card
- Load more: Infinite scroll or 'Load More' button
- View toggle: Grid/List icons top-right
",

            Models.ScreenType.ProductDetail => @"
PRODUCT DETAIL SCREEN:
- Image gallery: Full-width, swipeable, dots indicator
- Title: 24sp bold
- Price: 28sp bold, original price strike-through
- Rating: Stars + review count, tappable
- Color selector: Color swatches, selected state
- Size selector: Pills (S/M/L/XL), selected state
- Quantity: - [1] + stepper
- Add to Cart: Large button, 56px height, sticky bottom or after description
- Description: Expandable 'Read more' for long text
- Reviews section: Summary + top reviews
- Similar products: Horizontal scroll
",

            Models.ScreenType.Cart => @"
CART SCREEN:
- Header: 'Cart ([N] items)' count
- Item cards: Thumbnail 80x80, name, size/color, price
- Quantity stepper: - [N] + per item
- Remove: X icon or swipe to delete
- Subtotal: After each item or at bottom
- Promo code: Input field + Apply button
- Total: Large, bold, includes tax/shipping
- Checkout button: 56px height, full-width, sticky bottom
- Continue shopping: Link above footer
- Empty state: Illustration + 'Your cart is empty'
",

            Models.ScreenType.Checkout => @"
CHECKOUT SCREEN:
- Progress: Steps indicator (1.Cart → 2.Shipping → 3.Payment)
- Shipping address: Form or saved address selector
- Shipping method: Radio options (Standard, Express)
- Payment method: Card input or saved cards
- Order summary: Collapsible, items + total
- Apply coupon: Expandable section
- Place order: Large button, total price on button
- Security badges: SSL, secure checkout icons
- Terms: Checkbox 'I agree to terms'
",

            Models.ScreenType.Orders => @"
ORDERS SCREEN:
- Tabs: Active, Delivered, Cancelled
- Order cards: Order #, date, status badge
- Items: 2-3 product thumbnails, '+N more'
- Total: Bold, right-aligned
- Status: Color-coded (yellow=processing, green=delivered)
- Track button: For shipped orders
- Reorder: Quick reorder button
- Details: Tap card to expand or navigate
- Empty state: 'No orders yet' for each tab
",

            Models.ScreenType.PatientsList => @"
PATIENTS LIST SCREEN (HEALTHCARE):
- Search bar: 48px height, search icon, prominent top
- Filter chips: All, Critical, Monitoring, Stable
- Sort: Name, Recent, Status dropdown
- Patient cards: 88-104px height, repeating list
  - Photo: 48-64px circle, left-aligned, professional headshot
  - Name: 18sp bold, primary text
  - Age/Gender: 14sp gray below name ('45, M')
  - Status dot: 8px circle (green/yellow/red) before name
  - Condition tags: Pill chips below info (Diabetes, Hypertension)
  - Chevron: Right arrow for drill-down
  - Last visit: 12sp gray, right side
- FAB: + button, bottom-right, add new patient
- Dividers: 1px between cards or no divider
- Empty state: Illustration + 'No patients found'
- Pull to refresh
",

            Models.ScreenType.PatientDetail => @"
PATIENT DETAIL SCREEN (HEALTHCARE):
- Header: Patient name 24sp + status badge
- Photo: 96px circle, edit button overlay
- Info cards: Age, Blood type, Allergies with icons
- Vitals section: Heart rate, BP, temp in grid
- Tabs: History, Medications, Appointments, Documents
- Medical timeline: Vertical timeline with dates
- Visit cards: Date, doctor, diagnosis, notes
- Medications list: Name, dosage, frequency
- Action buttons: Schedule appointment, Add note, Call
- Emergency contact: Quick access card
",

            Models.ScreenType.Appointments => @"
APPOINTMENTS SCREEN (HEALTHCARE):
- Calendar view: Month/Week toggle, current date highlighted
- Upcoming section: Next 5 appointments, scrollable
- Appointment cards: 104px height
  - Date/Time: 24sp bold, top-left
  - Duration: 14sp below time
  - Patient: Name + 40px photo inline
  - Type: Video icon or in-person icon
  - Status: Upcoming (blue), Completed (green), Cancelled (gray)
  - Actions: Join call, Reschedule, Cancel
- FAB: + button to schedule new appointment
- Filter: All, Today, This week, Telemedicine
- Empty state: 'No appointments scheduled'
- Sync indicator: 'Synced with Google Calendar' with icon
",

            Models.ScreenType.CalendarSync => @"
CALENDAR SYNC SETTINGS SCREEN:
- Header: 'Calendar Integration' title
- Status card: Sync status with Google Calendar icon
- Connected account: Email, profile photo, disconnect button
- Sync options toggles:
  - Auto-sync appointments
  - Two-way sync
  - Sync reminders
  - Include patient names (privacy warning)
- Sync frequency: Dropdown (Real-time, Every 15min, Hourly)
- Calendar selection: Which calendar to sync to
- Last synced: Timestamp with refresh button
- Privacy notice: HIPAA compliance warning
- Connect button: If not connected, Google branded
- Sync now: Manual sync trigger button
",

            Models.ScreenType.Feed => @"
FEED SCREEN TEMPLATE:
- Pull-to-refresh at top
- Post cards: Repeating, 16px margin between
  - Avatar: 40px circle, top-lefts
  - Username: 16sp bold + timestamp 12sp gray
  - Content: Text 16sp + images full-width
  - Actions: Like, Comment, Share icons with counts
- Stories: Horizontal scroll at top, 64px avatars with rings
- Load more: Infinite scroll
- Filter tabs: Following, For You, Trending
",

            Models.ScreenType.Detail => @"
DETAIL SCREEN TEMPLATE:
- Back button: Top-left, 44x44px
- Hero image: Full-width, 16:9 or square
- Title: 28sp bold, 16px margins
- Metadata: Author, date, category chips
- Body content: 16sp, 1.5 line height, max-width 600px
- Media: Images, videos inline
- Related items: 'You might also like' section
- Actions: Share, Save, More (three-dot menu)
- Comments: Expandable section at bottom
",

            Models.ScreenType.Search => @"
SEARCH SCREEN TEMPLATE:
- Search bar: 56px height, auto-focus on load
- Voice search: Microphone icon right side
- Recent searches: List with clock icon, clear all option
- Suggestions: As-you-type results, highlighted matching text
- Filters: Category, date, price range chips
- Results: Grid or list based on content type
- No results: 'No results for [query]' + suggestions
- Clear button: X icon in search bar
",

            _ => "General screen layout with header, content area, and navigation."
        };
    }

    /// <summary>
    /// Get category-specific navigation tabs
    /// </summary>
    public static string GetCategoryTabs(string category)
    {
        return category.ToLowerInvariant() switch
        {
            "healthcare" => "Bottom tabs: Dashboard, Patients, Appointments, Messages, Profile",
            "ecommerce" => "Bottom tabs: Home, Categories, Cart, Orders, Account",
            "finance" => "Bottom tabs: Home, Accounts, Transactions, Budget, More",
            "social" => "Bottom tabs: Feed, Search, Post, Notifications, Profile",
            "fitness" => "Bottom tabs: Today, Workouts, Progress, Nutrition, Profile",
            "education" => "Bottom tabs: Courses, My Learning, Messages, Profile",
            "productivity" => "Bottom tabs: Tasks, Projects, Calendar, Team, Settings",
            _ => "Bottom tabs: Home, Browse, Activity, Profile"
        };
    }
}
