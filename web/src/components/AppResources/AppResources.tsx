import { useState } from 'react'
import { appCategories, screenTypeInfo } from '../../data/appCategories'
import type { AppCategory, Platform, ScreenType, User } from '../../types'
import { Smartphone, Monitor, Apple, Box, Check, Sparkles, Loader2 } from 'lucide-react'
import { ColorPicker } from '../IconGenerator/ColorPicker'
import { AppResourcesResults } from './AppResourcesResults'
import { api } from '../../services/api'
import toast from 'react-hot-toast'

// Category-specific color palette suggestions
const categoryColorPalettes: Record<AppCategory, Array<{ name: string; colors: string[] }>> = {
  'ecommerce': [
    { name: 'Trust Blue', colors: ['#0066FF', '#00D4FF'] },
    { name: 'Vibrant Sale', colors: ['#FF6B6B', '#FFE66D'] },
    { name: 'Luxury Gold', colors: ['#D4A574', '#8B7355'] },
    { name: 'Fresh Green', colors: ['#00B894', '#55EFC4'] },
  ],
  'healthcare': [
    { name: 'Medical Blue', colors: ['#4A90E2', '#50C878'] },
    { name: 'Calm Teal', colors: ['#00CEC9', '#81ECEC'] },
    { name: 'Trust Green', colors: ['#00B894', '#74B9FF'] },
    { name: 'Professional Navy', colors: ['#2D3E50', '#3498DB'] },
  ],
  'finance': [
    { name: 'Professional Blue', colors: ['#0066FF', '#004C99'] },
    { name: 'Wealth Gold', colors: ['#D4AF37', '#FFD700'] },
    { name: 'Trust Navy', colors: ['#1E3A8A', '#3B82F6'] },
    { name: 'Growth Green', colors: ['#059669', '#10B981'] },
  ],
  'social': [
    { name: 'Playful Purple', colors: ['#8B5CF6', '#EC4899'] },
    { name: 'Vibrant Gradient', colors: ['#FF3CAC', '#784BA0', '#2B86C5'] },
    { name: 'Warm Sunset', colors: ['#FF6B9D', '#FFE66D'] },
    { name: 'Cool Ocean', colors: ['#667EEA', '#764BA2'] },
  ],
  'fitness': [
    { name: 'Energy Red', colors: ['#EF4444', '#F97316'] },
    { name: 'Vitality Green', colors: ['#10B981', '#14B8A6'] },
    { name: 'Power Purple', colors: ['#8B5CF6', '#A855F7'] },
    { name: 'Active Orange', colors: ['#F59E0B', '#FB923C'] },
  ],
  'education': [
    { name: 'Knowledge Blue', colors: ['#3B82F6', '#60A5FA'] },
    { name: 'Growth Green', colors: ['#22C55E', '#4ADE80'] },
    { name: 'Creative Purple', colors: ['#A78BFA', '#C4B5FD'] },
    { name: 'Friendly Yellow', colors: ['#FBBF24', '#FCD34D'] },
  ],
  'productivity': [
    { name: 'Focus Blue', colors: ['#2563EB', '#3B82F6'] },
    { name: 'Calm Gray', colors: ['#6B7280', '#9CA3AF'] },
    { name: 'Efficient Teal', colors: ['#14B8A6', '#2DD4BF'] },
    { name: 'Balanced Purple', colors: ['#7C3AED', '#8B5CF6'] },
  ],
  'food': [
    { name: 'Appetizing Red', colors: ['#DC2626', '#EF4444'] },
    { name: 'Fresh Green', colors: ['#16A34A', '#22C55E'] },
    { name: 'Warm Orange', colors: ['#EA580C', '#F97316'] },
    { name: 'Gourmet Brown', colors: ['#78350F', '#92400E'] },
  ],
  'travel': [
    { name: 'Sky Blue', colors: ['#0EA5E9', '#38BDF8'] },
    { name: 'Adventure Orange', colors: ['#F97316', '#FB923C'] },
    { name: 'Tropical Green', colors: ['#059669', '#10B981'] },
    { name: 'Sunset Gradient', colors: ['#FF6B6B', '#FFE66D'] },
  ],
  'music': [
    { name: 'Rhythm Purple', colors: ['#7C3AED', '#A78BFA'] },
    { name: 'Beat Pink', colors: ['#DB2777', '#EC4899'] },
    { name: 'Sound Wave', colors: ['#0EA5E9', '#8B5CF6'] },
    { name: 'Neon Vibe', colors: ['#F59E0B', '#EC4899', '#8B5CF6'] },
  ],
  'custom': [
    { name: 'Professional Blue', colors: ['#3B82F6', '#60A5FA'] },
    { name: 'Creative Purple', colors: ['#8B5CF6', '#A855F7'] },
    { name: 'Modern Gradient', colors: ['#06B6D4', '#8B5CF6'] },
    { name: 'Classic Gray', colors: ['#6B7280', '#9CA3AF'] },
  ],
}

function getCategoryColorSuggestions(category: AppCategory) {
  return categoryColorPalettes[category] || categoryColorPalettes.ecommerce
}

interface AppResourcesProps {
  user?: User | null
  onUserUpdate?: () => void
}

export function AppResources({ user, onUserUpdate }: AppResourcesProps) {
  const [selectedCategory, setSelectedCategory] = useState<AppCategory | null>(null)
  const [selectedScreens, setSelectedScreens] = useState<ScreenType[]>([])
  const [selectedPlatforms, setSelectedPlatforms] = useState<Platform[]>(['ios', 'android'])
  const [appName, setAppName] = useState('')
  const [brandColors, setBrandColors] = useState(['#4A90E2', '#50C878'])

  // Generation state
  const [isGenerating, setIsGenerating] = useState(false)
  const [sessionId, setSessionId] = useState<string | null>(null)
  const [generatedScreens, setGeneratedScreens] = useState<any[]>([])
  const [showResults, setShowResults] = useState(false)

  const categoryInfo = selectedCategory ? appCategories.find(c => c.id === selectedCategory) : null

  const toggleScreen = (screen: ScreenType) => {
    setSelectedScreens(prev =>
      prev.includes(screen) ? prev.filter(s => s !== screen) : [...prev, screen]
    )
  }

  const togglePlatform = (platform: Platform) => {
    setSelectedPlatforms(prev =>
      prev.includes(platform) ? prev.filter(p => p !== platform) : [...prev, platform]
    )
  }

  const handleGenerate = async () => {
    if (!appName.trim()) {
      toast.error('Please enter an app name')
      return
    }

    if (!user) {
      toast.error('Please sign in to generate app resources')
      return
    }

    // Check credits
    const creditsNeeded = selectedScreens.length
    if (!user.isUnlimited && user.credits < creditsNeeded) {
      toast.error(`Not enough credits. You need ${creditsNeeded} credits but have ${user.credits}`)
      return
    }

    setIsGenerating(true)

    try {
      const request = {
        platforms: selectedPlatforms.map(p => p.charAt(0).toUpperCase() + p.slice(1)),
        options: {
          screenTypes: selectedScreens,
          appName: appName.trim(),
          appCategory: selectedCategory || 'app',
          brandPrimaryColor: brandColors[0],
          brandSecondaryColor: brandColors[1] || brandColors[0],
          targetPlatform: selectedPlatforms[0].charAt(0).toUpperCase() + selectedPlatforms[0].slice(1)
        }
      }

      const response = await api.generateAppResourcesV2(request)

      // Update state with results
      setSessionId(response.sessionId)
      setGeneratedScreens(response.screens || [])
      setShowResults(true)

      // Update user credits
      if (onUserUpdate) {
        onUserUpdate()
      }

      toast.success(`Generated ${response.screens?.length || 0} screens!`, {
        icon: '🎉',
        duration: 4000
      })

      // Scroll to results
      setTimeout(() => {
        window.scrollTo({ top: 0, behavior: 'smooth' })
      }, 100)
    } catch (error: any) {
      console.error('Generation error:', error)

      const errorMessage = error.response?.data?.error || error.message || 'Failed to generate app resources'
      toast.error(errorMessage, {
        duration: 5000
      })
    } finally {
      setIsGenerating(false)
    }
  }

  const platforms = [
    { id: 'ios' as Platform, name: 'iOS', icon: Apple, color: 'from-gray-800 to-gray-900', bgColor: 'bg-gray-900' },
    { id: 'android' as Platform, name: 'Android', icon: Smartphone, color: 'from-green-500 to-green-600', bgColor: 'bg-green-600' },
    { id: 'web' as Platform, name: 'Web/PWA', icon: Monitor, color: 'from-blue-500 to-blue-600', bgColor: 'bg-blue-600' },
    { id: 'macos' as Platform, name: 'macOS', icon: Box, color: 'from-purple-500 to-purple-600', bgColor: 'bg-purple-600' },
  ]

  // Show results view if generation is complete
  if (showResults && sessionId && generatedScreens.length > 0) {
    return (
      <AppResourcesResults
        sessionId={sessionId}
        screens={generatedScreens}
        appName={appName}
        appCategory={selectedCategory || 'app'}
        user={user}
        onUserUpdate={onUserUpdate}
      />
    )
  }

  return (
    <div className="container mx-auto px-4 py-8 md:py-12 max-w-7xl">
      {/* Hero Section */}
      <div className="text-center mb-8 md:mb-12 animate-slide-in">
        <div className="inline-flex items-center gap-2 bg-gradient-to-r from-blue-50 to-purple-50 border border-blue-200 text-blue-700 px-4 py-2 rounded-full mb-4 md:mb-6">
          <Sparkles size={16} />
          <span className="text-sm font-medium">AI-Powered App Design</span>
        </div>
        <h1 className="text-3xl md:text-5xl lg:text-6xl font-bold mb-4 md:mb-6 bg-gradient-to-r from-blue-600 via-purple-600 to-pink-600 bg-clip-text text-transparent">
          Generate Complete App Mockups
        </h1>
        <p className="text-lg md:text-xl text-gray-600 max-w-3xl mx-auto leading-relaxed">
          Create beautiful, platform-specific app designs with AI. Get screen mockups, icons, and design assets in seconds.
        </p>
      </div>

      {/* What You'll Get Preview */}
      <div className="bg-gradient-to-br from-blue-50 via-purple-50 to-pink-50 rounded-2xl shadow-lg p-6 md:p-8 mb-6 md:mb-8 border border-blue-100 animate-slide-in">
        <div className="text-center mb-6">
          <h2 className="text-2xl md:text-3xl font-bold mb-3 text-gray-900">What You'll Get</h2>
          <p className="text-gray-600 max-w-2xl mx-auto">Your complete app design package includes everything you need to launch</p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 md:gap-6">
          {/* App Icons */}
          <div className="bg-white rounded-xl p-5 shadow-md hover:shadow-xl transition-all duration-300 hover:scale-[1.02]">
            <div className="w-12 h-12 bg-gradient-to-br from-blue-500 to-blue-600 rounded-xl flex items-center justify-center mb-4 shadow-lg">
              <Apple size={24} className="text-white" />
            </div>
            <h3 className="font-bold text-lg mb-2 text-gray-900">App Icons</h3>
            <p className="text-sm text-gray-600 mb-3">20+ sizes for iOS, Android & Web</p>
            <div className="flex gap-2">
              <div className="w-8 h-8 bg-gradient-to-br from-blue-400 to-blue-600 rounded-lg shadow"></div>
              <div className="w-10 h-10 bg-gradient-to-br from-blue-400 to-blue-600 rounded-lg shadow"></div>
              <div className="w-12 h-12 bg-gradient-to-br from-blue-400 to-blue-600 rounded-xl shadow"></div>
            </div>
          </div>

          {/* Screen Mockups */}
          <div className="bg-white rounded-xl p-5 shadow-md hover:shadow-xl transition-all duration-300 hover:scale-[1.02]">
            <div className="w-12 h-12 bg-gradient-to-br from-purple-500 to-purple-600 rounded-xl flex items-center justify-center mb-4 shadow-lg">
              <Smartphone size={24} className="text-white" />
            </div>
            <h3 className="font-bold text-lg mb-2 text-gray-900">Screen Mockups</h3>
            <p className="text-sm text-gray-600 mb-3">Beautiful UI designs for each screen</p>
            <div className="grid grid-cols-3 gap-2">
              <div className="aspect-[9/16] bg-gradient-to-br from-gray-100 to-gray-200 rounded-lg shadow-inner"></div>
              <div className="aspect-[9/16] bg-gradient-to-br from-gray-100 to-gray-200 rounded-lg shadow-inner"></div>
              <div className="aspect-[9/16] bg-gradient-to-br from-gray-100 to-gray-200 rounded-lg shadow-inner"></div>
            </div>
          </div>

          {/* Design System */}
          <div className="bg-white rounded-xl p-5 shadow-md hover:shadow-xl transition-all duration-300 hover:scale-[1.02]">
            <div className="w-12 h-12 bg-gradient-to-br from-pink-500 to-pink-600 rounded-xl flex items-center justify-center mb-4 shadow-lg">
              <Sparkles size={24} className="text-white" />
            </div>
            <h3 className="font-bold text-lg mb-2 text-gray-900">Design System</h3>
            <p className="text-sm text-gray-600 mb-3">Colors, spacing & typography guide</p>
            <div className="flex gap-2">
              <div className="flex-1 h-8 bg-gradient-to-r from-blue-500 to-purple-500 rounded-lg shadow"></div>
              <div className="flex-1 h-8 bg-gradient-to-r from-pink-500 to-orange-500 rounded-lg shadow"></div>
            </div>
          </div>
        </div>
      </div>

      {/* Step 1: Category Selection */}
      <div className="bg-white rounded-2xl shadow-xl p-6 md:p-8 mb-6 md:mb-8 border border-gray-100 hover:shadow-2xl transition-shadow duration-300">
        <div className="flex items-center gap-3 mb-6">
          <div className="w-10 h-10 bg-gradient-to-br from-blue-500 to-blue-600 text-white rounded-full flex items-center justify-center font-bold shadow-lg">
            1
          </div>
          <h2 className="text-xl md:text-2xl font-bold text-gray-900">Choose Your App Category</h2>
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
          {appCategories.map(category => (
            <button
              key={category.id}
              onClick={() => setSelectedCategory(category.id)}
              className={`
                group relative p-6 rounded-2xl border-2 transition-all duration-300 text-left
                ${selectedCategory === category.id
                  ? 'border-blue-600 bg-blue-50 shadow-xl scale-[1.02] ring-2 ring-blue-200'
                  : 'border-gray-200 hover:border-blue-300 hover:shadow-lg hover:scale-[1.01]'}
              `}
            >
              {selectedCategory === category.id && (
                <div className="absolute top-3 right-3">
                  <div className="w-7 h-7 bg-blue-600 rounded-full flex items-center justify-center shadow-lg">
                    <Check size={16} className="text-white" />
                  </div>
                </div>
              )}

              <div className="text-5xl mb-3 group-hover:scale-110 transition-transform duration-300">{category.icon}</div>
              <h3 className="font-bold text-lg mb-2 text-gray-900">{category.name}</h3>
              <p className="text-sm text-gray-600 mb-3 line-clamp-2">{category.description}</p>
              <div className="flex flex-wrap gap-1.5">
                {category.features.slice(0, 2).map(feature => (
                  <span
                    key={feature}
                    className="text-xs px-2.5 py-1 bg-gray-100 rounded-full text-gray-700 font-medium"
                  >
                    {feature}
                  </span>
                ))}
              </div>
            </button>
          ))}
        </div>
      </div>

      {/* Step 2: App Details & Screen Selection */}
      {selectedCategory && categoryInfo && (
        <div className="bg-white rounded-2xl shadow-xl p-6 md:p-8 mb-6 md:mb-8 border border-gray-100 hover:shadow-2xl transition-shadow duration-300 animate-slide-in">
          <div className="flex items-center gap-3 mb-6">
            <div className="w-10 h-10 bg-gradient-to-br from-purple-500 to-purple-600 text-white rounded-full flex items-center justify-center font-bold shadow-lg">
              2
            </div>
            <h2 className="text-xl md:text-2xl font-bold text-gray-900">Configure Your App</h2>
          </div>

          {/* App Name & Colors */}
          <div className="mb-8">
            <h3 className="font-bold text-lg mb-4 text-gray-900">App Details</h3>
            <div className="grid grid-cols-1 gap-6">
              {/* App Name */}
              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-2">
                  App Name
                </label>
                <input
                  type="text"
                  value={appName}
                  onChange={(e) => setAppName(e.target.value)}
                  placeholder="e.g., ShopHub, HealthCare Pro, FitTracker"
                  className="input-premium w-full px-4 py-3 border-2 border-gray-300 rounded-xl focus:ring-4 focus:ring-blue-500 focus:ring-opacity-30 focus:border-blue-500 transition-all text-base font-medium"
                />
              </div>

              {/* Brand Colors - Enhanced with ColorPicker */}
              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-3">
                  Brand Colors
                </label>
                <div className="card-premium p-6 rounded-2xl">
                  {/* Category-specific suggestions */}
                  {selectedCategory && (
                    <div className="mb-5 p-4 bg-gradient-to-r from-blue-50 to-purple-50 rounded-xl border border-blue-100">
                      <p className="text-sm font-semibold text-gray-700 mb-2 flex items-center gap-2">
                        <span className="text-lg">{categoryInfo?.icon}</span>
                        Recommended for {categoryInfo?.name}
                      </p>
                      <div className="flex flex-wrap gap-2">
                        {getCategoryColorSuggestions(selectedCategory).map((suggestion, index) => (
                          <button
                            key={index}
                            onClick={() => setBrandColors(suggestion.colors)}
                            className="group px-3 py-2 bg-white rounded-lg border border-gray-200 hover:border-blue-400 hover:shadow-md transition-all"
                            title={`Apply ${suggestion.name} palette`}
                          >
                            <div className="flex gap-1 mb-1">
                              {suggestion.colors.map((color, i) => (
                                <div
                                  key={i}
                                  className="w-6 h-6 rounded"
                                  style={{ backgroundColor: color }}
                                />
                              ))}
                            </div>
                            <p className="text-xs font-medium text-gray-600 group-hover:text-blue-600">
                              {suggestion.name}
                            </p>
                          </button>
                        ))}
                      </div>
                    </div>
                  )}

                  <ColorPicker colors={brandColors} onChange={setBrandColors} />
                  <p className="mt-4 text-xs text-gray-500 flex items-center gap-2">
                    <Sparkles className="w-3 h-3" />
                    Choose colors that match your brand identity. Click a color to customize or select a preset palette.
                  </p>
                </div>
              </div>
            </div>
          </div>

          {/* Screen Selection */}
          <div>
            <h3 className="font-bold text-lg mb-4 text-gray-900">
              Select Screens 
              <span className="ml-2 text-sm font-normal text-blue-600 bg-blue-50 px-3 py-1 rounded-full">
                {selectedScreens.length} selected
              </span>
            </h3>
            <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-3">
              {categoryInfo.screens.map(screen => {
                const info = screenTypeInfo[screen]
                const isSelected = selectedScreens.includes(screen)

                return (
                  <button
                    key={screen}
                    onClick={() => toggleScreen(screen)}
                    className={`
                      group relative p-4 rounded-xl border-2 transition-all duration-300 text-left
                      ${isSelected
                        ? 'border-blue-600 bg-blue-50 shadow-lg scale-[1.02]'
                        : 'border-gray-200 hover:border-blue-300 hover:shadow-md hover:scale-[1.01]'}
                    `}
                  >
                    {isSelected && (
                      <div className="absolute top-2 right-2">
                        <div className="w-6 h-6 bg-blue-600 rounded-full flex items-center justify-center shadow-md">
                          <Check size={14} className="text-white" />
                        </div>
                      </div>
                    )}

                    <div className="text-3xl mb-2 group-hover:scale-110 transition-transform duration-300">{info.icon}</div>
                    <div className="font-semibold text-sm mb-1 text-gray-900">{info.name}</div>
                    <div className="text-xs text-gray-600 line-clamp-2">{info.description}</div>
                  </button>
                )
              })}
            </div>
          </div>
        </div>
      )}

      {/* Step 3: Platform Selection */}
      {selectedCategory && selectedScreens.length > 0 && (
        <div className="bg-white rounded-2xl shadow-xl p-6 md:p-8 mb-6 md:mb-8 border border-gray-100 hover:shadow-2xl transition-shadow duration-300 animate-slide-in">
          <div className="flex items-center gap-3 mb-6">
            <div className="w-10 h-10 bg-gradient-to-br from-pink-500 to-pink-600 text-white rounded-full flex items-center justify-center font-bold shadow-lg">
              3
            </div>
            <h2 className="text-xl md:text-2xl font-bold text-gray-900">Select Platforms</h2>
          </div>

          <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
            {platforms.map(platform => {
              const isSelected = selectedPlatforms.includes(platform.id)
              const Icon = platform.icon

              return (
                <button
                  key={platform.id}
                  onClick={() => togglePlatform(platform.id)}
                  className={`
                    group relative p-6 rounded-2xl border-2 transition-all duration-300
                    ${isSelected
                      ? 'border-blue-600 bg-blue-50 shadow-xl scale-[1.05]'
                      : 'border-gray-200 hover:border-blue-300 hover:shadow-lg hover:scale-[1.02]'}
                  `}
                >
                  {isSelected && (
                    <div className="absolute top-3 right-3">
                      <div className="w-7 h-7 bg-blue-600 rounded-full flex items-center justify-center shadow-lg">
                        <Check size={16} className="text-white" />
                      </div>
                    </div>
                  )}

                  <div className={`w-16 h-16 bg-gradient-to-br ${platform.color} rounded-2xl flex items-center justify-center mb-3 shadow-lg group-hover:scale-110 transition-transform duration-300`}>
                    <Icon size={28} className="text-white" />
                  </div>
                  <div className="font-bold text-base text-gray-900">{platform.name}</div>
                </button>
              )
            })}
          </div>
        </div>
      )}

      {/* Generate Button */}
      {selectedCategory && selectedScreens.length > 0 && selectedPlatforms.length > 0 && (
        <div className="bg-gradient-to-br from-blue-600 via-purple-600 to-pink-600 rounded-2xl shadow-2xl p-6 md:p-8 text-white animate-scale-in">
          <div className="flex flex-col md:flex-row items-center justify-between gap-6">
            <div>
              <h3 className="text-2xl md:text-3xl font-bold mb-2">Ready to Generate! 🎨</h3>
              <p className="text-blue-100 text-lg">
                {selectedScreens.length} screens × {selectedPlatforms.length} platforms = <span className="font-bold text-white">{selectedScreens.length * selectedPlatforms.length} assets</span>
              </p>
              <p className="text-sm text-blue-200 mt-2 flex items-center gap-2">
                <span>💎</span>
                Estimated cost: {selectedScreens.length * 0.04} credits (~${(selectedScreens.length * 0.04).toFixed(2)})
              </p>
            </div>
            <button
              onClick={handleGenerate}
              disabled={isGenerating || !appName.trim()}
              className="w-full md:w-auto bg-white text-blue-600 px-8 py-4 rounded-xl font-bold text-lg hover:bg-blue-50 hover:scale-105 transition-all duration-300 flex items-center justify-center gap-2 shadow-2xl disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:scale-100"
            >
              {isGenerating ? (
                <>
                  <Loader2 size={24} className="animate-spin" />
                  Generating...
                </>
              ) : (
                <>
                  <Sparkles size={24} />
                  Generate App Mockups
                </>
              )}
            </button>
          </div>
        </div>
      )}

      {/* Empty State */}
      {!selectedCategory && (
        <div className="text-center py-20 text-gray-400 animate-float">
          <div className="text-8xl mb-6">🎨</div>
          <p className="text-xl font-medium text-gray-500">Select an app category to get started</p>
          <p className="text-sm text-gray-400 mt-2">Choose from our curated categories above</p>
        </div>
      )}
    </div>
  )
}
