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
    { id: 'ios' as Platform, name: 'iOS', icon: Apple, color: 'bg-gray-900' },
    { id: 'android' as Platform, name: 'Android', icon: Smartphone, color: 'bg-green-600' },
    { id: 'web' as Platform, name: 'Web/PWA', icon: Monitor, color: 'bg-blue-600' },
    { id: 'macos' as Platform, name: 'macOS', icon: Box, color: 'bg-purple-600' },
  ]

  return (
    <div className="container mx-auto px-4 py-12 max-w-7xl">
      {/* Hero Section */}
      <div className="text-center mb-12">
        <div className="inline-flex items-center gap-2 bg-blue-100 text-blue-700 px-4 py-2 rounded-full mb-4">
          <Sparkles size={16} />
          <span className="text-sm font-medium">AI-Powered App Design</span>
        </div>
        <h1 className="text-5xl font-bold mb-4 bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">
          Generate Complete App Mockups
        </h1>
        <p className="text-xl text-gray-600 max-w-2xl mx-auto">
          Create beautiful, platform-specific app designs with AI. Get screen mockups, icons, and design assets in seconds.
        </p>
      </div>

      {/* Step 1: Category Selection */}
      <div className="bg-white rounded-xl shadow-lg p-8 mb-8">
        <div className="flex items-center gap-3 mb-6">
          <div className="w-8 h-8 bg-blue-600 text-white rounded-full flex items-center justify-center font-bold">
            1
          </div>
          <h2 className="text-2xl font-bold">Choose Your App Category</h2>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
          {appCategories.map(category => (
            <button
              key={category.id}
              onClick={() => setSelectedCategory(category.id)}
              className={`
                relative p-6 rounded-xl border-2 transition-all text-left
                ${selectedCategory === category.id
                  ? 'border-blue-600 bg-blue-50 shadow-lg scale-105'
                  : 'border-gray-200 hover:border-blue-300 hover:shadow-md'}
              `}
            >
              {selectedCategory === category.id && (
                <div className="absolute top-3 right-3">
                  <div className="w-6 h-6 bg-blue-600 rounded-full flex items-center justify-center">
                    <Check size={14} className="text-white" />
                  </div>
                </div>
              )}

              <div className="text-4xl mb-3">{category.icon}</div>
              <h3 className="font-bold text-lg mb-2">{category.name}</h3>
              <p className="text-sm text-gray-600 mb-3">{category.description}</p>
              <div className="flex flex-wrap gap-1">
                {category.features.slice(0, 2).map(feature => (
                  <span
                    key={feature}
                    className="text-xs px-2 py-1 bg-gray-100 rounded-full text-gray-600"
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
        <div className="bg-white rounded-xl shadow-lg p-8 mb-8">
          <div className="flex items-center gap-3 mb-6">
            <div className="w-8 h-8 bg-blue-600 text-white rounded-full flex items-center justify-center font-bold">
              2
            </div>
            <h2 className="text-2xl font-bold">Configure Your App</h2>
          </div>

          {/* App Name & Colors */}
          <div className="mb-8">
            <h3 className="font-semibold mb-4">App Details</h3>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  App Name
                </label>
                <input
                  type="text"
                  value={appName}
                  onChange={(e) => setAppName(e.target.value)}
                  placeholder="e.g., ShopHub, HealthCare Pro"
                  className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-2">
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
                        className="w-12 h-12 rounded-lg cursor-pointer border-2 border-gray-300"
                      />
                    </div>
                  ))}
                  {brandColors.length < 3 && (
                    <button
                      onClick={() => setBrandColors([...brandColors, '#000000'])}
                      className="w-12 h-12 border-2 border-dashed border-gray-300 rounded-lg text-gray-400 hover:border-blue-500 hover:text-blue-500"
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
            <h3 className="font-semibold mb-4">
              Select Screens ({selectedScreens.length} selected)
            </h3>
            <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-3">
              {categoryInfo.screens.map(screen => {
                const info = screenTypeInfo[screen]
                const isSelected = selectedScreens.includes(screen)

                return (
                  <button
                    key={screen}
                    onClick={() => toggleScreen(screen)}
                    className={`
                      relative p-4 rounded-lg border-2 transition-all text-left
                      ${isSelected
                        ? 'border-blue-600 bg-blue-50'
                        : 'border-gray-200 hover:border-blue-300'}
                    `}
                  >
                    {isSelected && (
                      <div className="absolute top-2 right-2">
                        <div className="w-5 h-5 bg-blue-600 rounded-full flex items-center justify-center">
                          <Check size={12} className="text-white" />
                        </div>
                      </div>
                    )}

                    <div className="text-2xl mb-2">{info.icon}</div>
                    <div className="font-medium text-sm mb-1">{info.name}</div>
                    <div className="text-xs text-gray-500">{info.description}</div>
                  </button>
                )
              })}
            </div>
          </div>
        </div>
      )}

      {/* Step 3: Platform Selection */}
      {selectedCategory && selectedScreens.length > 0 && (
        <div className="bg-white rounded-xl shadow-lg p-8 mb-8">
          <div className="flex items-center gap-3 mb-6">
            <div className="w-8 h-8 bg-blue-600 text-white rounded-full flex items-center justify-center font-bold">
              3
            </div>
            <h2 className="text-2xl font-bold">Select Platforms</h2>
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
                    relative p-6 rounded-xl border-2 transition-all
                    ${isSelected
                      ? 'border-blue-600 bg-blue-50 shadow-lg'
                      : 'border-gray-200 hover:border-blue-300'}
                  `}
                >
                  {isSelected && (
                    <div className="absolute top-3 right-3">
                      <div className="w-6 h-6 bg-blue-600 rounded-full flex items-center justify-center">
                        <Check size={14} className="text-white" />
                      </div>
                    </div>
                  )}

                  <div className={`w-12 h-12 ${platform.color} rounded-lg flex items-center justify-center mb-3`}>
                    <Icon size={24} className="text-white" />
                  </div>
                  <div className="font-bold">{platform.name}</div>
                </button>
              )
            })}
          </div>
        </div>
      )}

      {/* Generate Button */}
      {selectedCategory && selectedScreens.length > 0 && selectedPlatforms.length > 0 && (
        <div className="bg-gradient-to-r from-blue-600 to-purple-600 rounded-xl shadow-xl p-8 text-white">
          <div className="flex flex-col md:flex-row items-center justify-between gap-6">
            <div>
              <h3 className="text-2xl font-bold mb-2">Ready to Generate!</h3>
              <p className="text-blue-100">
                {selectedScreens.length} screens × {selectedPlatforms.length} platforms = {selectedScreens.length * selectedPlatforms.length} assets
              </p>
              <p className="text-sm text-blue-200 mt-1">
                Estimated cost: {selectedScreens.length * 0.04} credits (~${(selectedScreens.length * 0.04).toFixed(2)})
              </p>
            </div>
            <button
              className="bg-white text-blue-600 px-8 py-4 rounded-lg font-bold text-lg hover:bg-blue-50 transition-colors flex items-center gap-2 shadow-lg"
            >
              <Sparkles size={20} />
              Generate App Mockups
            </button>
          </div>
        </div>
      )}

      {/* Empty State */}
      {!selectedCategory && (
        <div className="text-center py-16 text-gray-400">
          <div className="text-6xl mb-4">🎨</div>
          <p className="text-lg">Select an app category to get started</p>
        </div>
      )}
    </div>
  )
}
