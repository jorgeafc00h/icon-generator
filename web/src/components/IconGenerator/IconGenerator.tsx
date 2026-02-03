import { useState } from 'react'
import { useMutation } from '@tanstack/react-query'
import { api } from '../../services/api'
import { StyleSelector } from './StyleSelector'
import { ColorPicker } from './ColorPicker'
import { PromptInput } from './PromptInput'
import { GenerationResults } from './GenerationResults'
import { Sparkles, Wand2, Palette, Zap } from 'lucide-react'
import toast from 'react-hot-toast'
import type { IconGenerationRequest, IconStyle } from '../../types'

export function IconGenerator() {
  const [prompt, setPrompt] = useState('')
  const [selectedStyle, setSelectedStyle] = useState<IconStyle>('3D')
  const [colors, setColors] = useState<string[]>(['#667EEA', '#764BA2'])
  const [quality, setQuality] = useState<'standard' | 'hd'>('standard')
  const [generatedIcon, setGeneratedIcon] = useState<any>(null)

  const generateMutation = useMutation({
    mutationFn: (request: IconGenerationRequest) => api.generateIcon(request),
    onSuccess: (data) => {
      setGeneratedIcon(data)
      toast.success('Icon generated successfully!')
    },
    onError: (error: any) => {
      toast.error(error.response?.data?.message || 'Failed to generate icon')
    },
  })

  const handleGenerate = () => {
    if (!prompt.trim()) {
      toast.error('Please describe your icon')
      return
    }

    if (!selectedStyle) {
      toast.error('Please select a style')
      return
    }

    generateMutation.mutate({
      keywords: prompt,
      style: selectedStyle,
      colors,
      quality,
    })
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 md:py-12">
      {/* Hero Section */}
      <div className="text-center mb-8 md:mb-12">
        <div className="inline-flex items-center gap-2 bg-gradient-to-r from-blue-50 to-purple-50 border border-blue-200 text-blue-700 px-4 py-2 rounded-full text-sm font-medium mb-4 md:mb-6">
          <Zap className="w-4 h-4" />
          AI-Powered Icon Generation
        </div>

        <h1 className="text-3xl md:text-5xl lg:text-6xl font-bold mb-4 md:mb-6 bg-gradient-to-r from-blue-600 via-purple-600 to-pink-600 bg-clip-text text-transparent">
          Create Stunning App Icons
        </h1>

        <p className="text-lg md:text-xl text-gray-600 max-w-2xl mx-auto mb-6 md:mb-8">
          Generate professional, unique app icons in seconds using AI.
          Perfect for iOS, Android, and web applications.
        </p>

        <div className="flex flex-wrap items-center justify-center gap-4 md:gap-8 text-sm text-gray-500">
          <div className="flex items-center gap-2">
            <Sparkles className="w-5 h-5 text-yellow-500" />
            <span>18+ Styles</span>
          </div>
          <div className="flex items-center gap-2">
            <Palette className="w-5 h-5 text-purple-500" />
            <span>Custom Colors</span>
          </div>
          <div className="flex items-center gap-2">
            <Wand2 className="w-5 h-5 text-blue-500" />
            <span>AI Enhanced</span>
          </div>
        </div>
      </div>

      {/* Main Generator */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 lg:gap-8">
        {/* Left Column - Input (2 columns on large screens) */}
        <div className="lg:col-span-2 space-y-4 md:space-y-6">
          {/* Step 1: Describe */}
          <div className="bg-white rounded-2xl shadow-lg p-4 md:p-6 border border-gray-100 hover:shadow-xl transition-shadow duration-300">
            <div className="flex items-center gap-3 mb-4">
              <div className="w-8 h-8 bg-gradient-to-br from-blue-500 to-blue-600 text-white rounded-full flex items-center justify-center font-bold text-sm shadow-lg">
                1
              </div>
              <h2 className="text-lg md:text-xl font-bold text-gray-900">Describe Your Icon</h2>
            </div>

            <PromptInput
              value={prompt}
              onChange={setPrompt}
              onGenerate={handleGenerate}
              isGenerating={generateMutation.isPending}
            />
          </div>

          {/* Step 2: Choose Style */}
          <div className="bg-white rounded-2xl shadow-lg p-4 md:p-6 border border-gray-100 hover:shadow-xl transition-shadow duration-300">
            <div className="flex items-center gap-3 mb-4">
              <div className="w-8 h-8 bg-gradient-to-br from-purple-500 to-purple-600 text-white rounded-full flex items-center justify-center font-bold text-sm shadow-lg">
                2
              </div>
              <h2 className="text-lg md:text-xl font-bold text-gray-900">Select Style</h2>
            </div>

            <StyleSelector
              selectedStyle={selectedStyle}
              onSelectStyle={setSelectedStyle}
            />
          </div>

          {/* Step 3: Choose Colors */}
          <div className="bg-white rounded-2xl shadow-lg p-4 md:p-6 border border-gray-100 hover:shadow-xl transition-shadow duration-300">
            <div className="flex items-center gap-3 mb-4">
              <div className="w-8 h-8 bg-gradient-to-br from-pink-500 to-pink-600 text-white rounded-full flex items-center justify-center font-bold text-sm shadow-lg">
                3
              </div>
              <h2 className="text-lg md:text-xl font-bold text-gray-900">Choose Colors</h2>
            </div>

            <ColorPicker colors={colors} onChange={setColors} />
          </div>
        </div>

        {/* Right Column - Results & Actions (1 column on large screens) */}
        <div className="space-y-4 md:space-y-6 lg:sticky lg:top-6 h-fit">
          {/* Preview */}
          <GenerationResults
            icon={generatedIcon}
            isGenerating={generateMutation.isPending}
          />

          {/* Preview */}
          <GenerationResults
            icon={generatedIcon}
            isGenerating={generateMutation.isPending}
          />

          {/* Quality & Generate */}
          <div className="bg-gradient-to-br from-blue-50 via-purple-50 to-pink-50 rounded-2xl p-4 md:p-6 border-2 border-blue-200 shadow-lg">
            <div className="space-y-4">
              {/* Quality Selector */}
              <div>
                <div className="flex items-center justify-between mb-3">
                  <div>
                    <h3 className="font-semibold text-gray-900 text-sm md:text-base">Image Quality</h3>
                    <p className="text-xs md:text-sm text-gray-600">HD costs 2 credits</p>
                  </div>
                  <div className="flex gap-2">
                    <button
                      onClick={() => setQuality('standard')}
                      className={`px-3 md:px-4 py-2 rounded-lg font-medium transition-all text-sm ${
                        quality === 'standard'
                          ? 'bg-white text-gray-900 shadow-md ring-2 ring-blue-500'
                          : 'text-gray-600 hover:text-gray-900 hover:bg-white/50'
                      }`}
                    >
                      Standard
                    </button>
                    <button
                      onClick={() => setQuality('hd')}
                      className={`px-3 md:px-4 py-2 rounded-lg font-medium transition-all text-sm ${
                        quality === 'hd'
                          ? 'bg-gradient-to-r from-yellow-400 to-orange-400 text-white shadow-md'
                          : 'text-gray-600 hover:text-gray-900 hover:bg-white/50'
                      }`}
                    >
                      HD ⚡
                    </button>
                  </div>
                </div>
              </div>

              {/* Generate Button */}
              <button
                onClick={handleGenerate}
                disabled={generateMutation.isPending || !prompt.trim()}
                className="w-full bg-gradient-to-r from-blue-600 via-purple-600 to-pink-600 text-white font-bold py-3 md:py-4 px-6 md:px-8 rounded-xl shadow-xl hover:shadow-2xl hover:scale-[1.02] transition-all disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:scale-100 flex items-center justify-center gap-3 text-base md:text-lg"
              >
                {generateMutation.isPending ? (
                  <>
                    <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin" />
                    Generating...
                  </>
                ) : (
                  <>
                    <Wand2 className="w-5 h-5 md:w-6 md:h-6" />
                    Generate Icon
                  </>
                )}
              </button>

              <p className="text-center text-xs md:text-sm text-gray-600 font-medium">
                💎 {quality === 'standard' ? '1 credit' : '2 credits'} per generation
              </p>
            </div>
          </div>
        </div>
      </div>

      {/* Features Section */}
      <div className="mt-12 md:mt-20 grid grid-cols-1 md:grid-cols-3 gap-6 md:gap-8">
        {[
          {
            icon: Sparkles,
            title: 'AI-Powered',
            description: 'Advanced DALL-E 3 technology creates unique, professional icons',
            gradient: 'from-blue-500 to-cyan-500',
          },
          {
            icon: Palette,
            title: 'Fully Customizable',
            description: 'Choose from 18+ styles and custom color combinations',
            gradient: 'from-purple-500 to-pink-500',
          },
          {
            icon: Wand2,
            title: 'Instant Results',
            description: 'Generate production-ready icons in seconds, not hours',
            gradient: 'from-pink-500 to-rose-500',
          },
        ].map((feature) => {
          const Icon = feature.icon
          return (
            <div
              key={feature.title}
              className="group bg-white rounded-2xl p-6 border border-gray-100 hover:shadow-xl hover:scale-[1.02] transition-all duration-300"
            >
              <div className={`w-12 h-12 bg-gradient-to-br ${feature.gradient} rounded-xl flex items-center justify-center mb-4 shadow-lg group-hover:scale-110 transition-transform`}>
                <Icon className="w-6 h-6 text-white" />
              </div>
              <h3 className="text-lg font-semibold mb-2 text-gray-900">{feature.title}</h3>
              <p className="text-gray-600 text-sm leading-relaxed">{feature.description}</p>
            </div>
          )
        })}
      </div>
    </div>
  )
}
