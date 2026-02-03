import { useState } from 'react'
import { appCategories, screenTypeInfo } from '../../data/appCategories'
import type { AppCategory, Platform, ScreenType } from '../../types'
import { Smartphone, Monitor, Apple, Box, Check, Sparkles } from 'lucide-react'

export function AppResources() {
  const [selectedCategory, setSelectedCategory] = useState<AppCategory | null>(null)
  const [selectedScreens, setSelectedScreens] = useState<ScreenType[]>([])
  const [selectedPlatforms, setSelectedPlatforms] = useState<Platform[]>(['ios', 'android'])
  const [appName, setAppName] = useState('')
  const [brandColors, setBrandColors] = useState(['#4A90E2', '#50C878'])

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

  const platforms = [
    { id: 'ios' as Platform, name: 'iOS', icon: Apple, color: 'from-gray-800 to-gray-900', bgColor: 'bg-gray-900' },
    { id: 'android' as Platform, name: 'Android', icon: Smartphone, color: 'from-green-500 to-green-600', bgColor: 'bg-green-600' },
    { id: 'web' as Platform, name: 'Web/PWA', icon: Monitor, color: 'from-blue-500 to-blue-600', bgColor: 'bg-blue-600' },
    { id: 'macos' as Platform, name: 'macOS', icon: Box, color: 'from-purple-500 to-purple-600', bgColor: 'bg-purple-600' },
  ]

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
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-2">
                  App Name
                </label>
                <input
                  type="text"
                  value={appName}
                  onChange={(e) => setAppName(e.target.value)}
                  placeholder="e.g., ShopHub, HealthCare Pro"
                  className="w-full px-4 py-3 border-2 border-gray-300 rounded-xl focus:ring-4 focus:ring-blue-500 focus:ring-opacity-30 focus:border-blue-500 transition-all"
                />
              </div>
              <div>
                <label className="block text-sm font-semibold text-gray-700 mb-2">
                  Brand Colors
                </label>
                <div className="flex gap-3">
                  {brandColors.map((color, index) => (
                    <div key={index} className="flex items-center gap-2">
                      <input
                        type="color"
                        value={color}
                        onChange={(e) => {
                          const newColors = [...brandColors]
                          newColors[index] = e.target.value
                          setBrandColors(newColors)
                        }}
                        className="w-14 h-14 rounded-xl cursor-pointer border-4 border-white shadow-lg hover:scale-110 transition-transform"
                      />
                    </div>
                  ))}
                  {brandColors.length < 3 && (
                    <button
                      onClick={() => setBrandColors([...brandColors, '#000000'])}
                      className="w-14 h-14 border-2 border-dashed border-gray-300 rounded-xl text-gray-400 hover:border-blue-500 hover:text-blue-500 hover:scale-110 transition-all font-bold text-xl"
                    >
                      +
                    </button>
                  )}
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
              className="w-full md:w-auto bg-white text-blue-600 px-8 py-4 rounded-xl font-bold text-lg hover:bg-blue-50 hover:scale-105 transition-all duration-300 flex items-center justify-center gap-2 shadow-2xl"
            >
              <Sparkles size={24} />
              Generate App Mockups
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
