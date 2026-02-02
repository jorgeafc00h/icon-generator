// Icon Generation Types
export interface IconGenerationRequest {
  keywords: string
  style: string
  colors: string[]
  quality?: 'standard' | 'hd'
}

export interface IconGenerationResponse {
  iconId: string
  imageUrl: string
  enhancedPrompt: string
  creditsRemaining: number
}

// App Resources Types
export interface AppResourcesRequest {
  iconId: string
  platforms: Platform[]
  options: AppResourcesOptions
}

export type Platform = 'ios' | 'android' | 'web' | 'macos'

export interface AppResourcesOptions {
  includeAdaptiveIcons?: boolean
  generateAppIconSet?: boolean
  optimizeForClarity?: boolean
  generateDarkMode?: boolean
  generateMaterialYou?: boolean
  backgroundColor?: string
  appName?: string
  themeColor?: string
}

export interface AppResourcesResponse {
  id: string
  zipUrl: string
  designGuideUrl?: string
  expiresAt: string
  fileSize: number
  totalAssets: number
  platforms: Platform[]
  designScore?: DesignQualityScore
  extractedColors?: ColorPalette
}

// Design Quality
export interface DesignQualityScore {
  overallScore: number
  clarityScore: number
  contrastScore: number
  scalabilityScore: number
  accessibilityScore: number
  meetsWCAG_AA: boolean
  meetsWCAG_AAA: boolean
  issues: DesignIssue[]
  suggestions: DesignSuggestion[]
}

export interface DesignIssue {
  severity: 'info' | 'warning' | 'critical'
  category: 'accessibility' | 'clarity' | 'platform' | 'contrast' | 'scalability'
  message: string
  fix: string
}

export interface DesignSuggestion {
  title: string
  description: string
  impact: 'low' | 'medium' | 'high'
  effort: 'low' | 'medium' | 'high'
  category: string
}

// Color Palette
export interface ColorPalette {
  dominant: string
  vibrant: string
  darkVibrant: string
  lightVibrant: string
  muted: string
  darkMuted: string
  lightMuted: string
  materialYou?: MaterialYouScheme
}

export interface MaterialYouScheme {
  primary: string
  onPrimary: string
  primaryContainer: string
  secondary: string
  tertiary: string
  error: string
  background: string
  surface: string
}

// Style Options
export type IconStyle =
  | '3D'
  | 'Minimal'
  | 'Gradient'
  | 'Glassmorphism'
  | 'Neomorphism'
  | 'Clay'
  | 'Pixel'
  | 'Flat'
  | 'Isometric'
  | 'Hand-drawn'
  | 'Geometric'
  | 'Abstract'
  | 'Retro'
  | 'Neon'
  | 'Watercolor'
  | 'Metallic'
  | 'Cartoon'
  | 'Realistic'

export interface StyleOption {
  id: IconStyle
  name: string
  description: string
  thumbnail?: string
  popular?: boolean
}

// User & Credits
export interface User {
  id: string
  email: string
  name?: string
  profilePictureUrl?: string
  credits: number
  createdAt: string
  updatedAt: string
  auth?: UserAuth
  metadata?: UserMetadata
  preferences?: UserPreferences
}

export interface UserAuth {
  googleId?: string
  googleEmail?: string
  lastLoginAt?: string
}

export interface UserMetadata {
  lastIconGenerated?: string
  totalIconsGenerated: number
  totalCreditsPurchased: number
  totalCreditsSpent: number
}

export interface UserPreferences {
  defaultStyle?: string
  favoriteColors?: string[]
  defaultQuality?: 'standard' | 'hd'
  emailNotifications: boolean
}

export interface IconGeneration {
  id: string
  userId: string
  prompt: string
  enhancedPrompt: string
  style: string
  colors: string[]
  imageUrl: string
  quality: string
  createdAt: string
}

// Screen Mockup Generation
export type ScreenType =
  | 'login'
  | 'signup'
  | 'home'
  | 'dashboard'
  | 'profile'
  | 'settings'
  | 'product-list'
  | 'product-detail'
  | 'cart'
  | 'checkout'
  | 'orders'
  | 'patients-list'
  | 'patient-detail'
  | 'appointments'
  | 'calendar-sync'

export type AppCategory =
  | 'ecommerce'
  | 'healthcare'
  | 'fitness'
  | 'education'
  | 'finance'
  | 'social'
  | 'productivity'
  | 'travel'
  | 'food'
  | 'music'
  | 'custom'

export interface AppCategoryInfo {
  id: AppCategory
  name: string
  description: string
  icon: string
  color: string
  screens: ScreenType[]
  features: string[]
}

export interface ScreenMockupRequest {
  appName: string
  category: AppCategory
  brandColors: string[]
  screens: ScreenType[]
  style?: 'Modern' | 'Minimal' | 'Material'
  quality?: 'standard' | 'hd'
}

export interface ScreenMockupResponse {
  id: string
  screens: ScreenMockupResult[]
  totalCost: number
  expiresAt: string
}

export interface ScreenMockupResult {
  screenType: ScreenType
  imageUrl: string
  prompt: string
  designScore?: DesignQualityScore
}

// Authentication
export interface GoogleAuthRequest {
  idToken: string
}

export interface AuthResponse {
  userId: string
  email: string
  name?: string
  profilePictureUrl?: string
  credits: number
  accessToken: string
  isNewUser: boolean
}

// Transactions
export interface Transaction {
  id: string
  type: 'purchase' | 'usage'
  credits: number
  amountInCents?: number
  description: string
  createdAt: string
}

// Payments
export interface CreditPackage {
  id: string
  name: string
  credits: number
  priceInCents: number
  stripePriceId?: string
  popular?: boolean
}

export interface PricingPlan {
  id: string
  name: string
  credits: number
  price: number
  popular?: boolean
  features: string[]
}

// User Data
export interface UserData {
  user: User
  icons: IconGeneration[]
  transactions: Transaction[]
}
