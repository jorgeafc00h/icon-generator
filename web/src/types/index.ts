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
  creditsRemaining: number
  generatedIcons: IconGeneration[]
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

// Payments
export interface PricingPlan {
  id: string
  name: string
  credits: number
  price: number
  popular?: boolean
  features: string[]
}
