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
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      {/* Hero Section */}
      <div className="text-center mb-12">
        <div className="inline-flex items-center gap-2 bg-blue-100 text-blue-700 px-4 py-2 rounded-full text-sm font-medium mb-6 animate-float">
          <Zap className="w-4 h-4" />
          AI-Powered Icon Generation
        </div>

        <h1 className="text-4xl md:text-6xl font-bold mb-6 bg-gradient-to-r from-blue-600 via-purple-600 to-pink-600 bg-clip-text text-transparent">
          Create Stunning App Icons
        </h1>

        <p className="text-xl text-gray-600 max-w-2xl mx-auto mb-8">
          Generate professional, unique app icons in seconds using AI.
          Perfect for iOS, Android, and web applications.
        </p>

        <div className="flex items-center justify-center gap-8 text-sm text-gray-500">
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
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
        {/* Left Column - Input */}
        <div className="space-y-6">
          {/* Step 1: Describe */}
          <div className="bg-white rounded-2xl shadow-lg p-6 border border-gray-100">
            <div className="flex items-center gap-3 mb-4">
              <div className="w-8 h-8 bg-blue-100 text-blue-600 rounded-full flex items-center justify-center font-bold">
                1
              </div>
              <h2 className="text-xl font-bold">Describe Your Icon</h2>
            </div>

            <PromptInput
              value={prompt}
              onChange={setPrompt}
              onGenerate={handleGenerate}
              isGenerating={generateMutation.isPending}
            />
          </div>

          {/* Step 2: Choose Colors */}
          <div className="bg-white rounded-2xl shadow-lg p-6 border border-gray-100">
            <div className="flex items-center gap-3 mb-4">
              <div className="w-8 h-8 bg-purple-100 text-purple-600 rounded-full flex items-center justify-center font-bold">
                2
              </div>
              <h2 className="text-xl font-bold">Choose Colors</h2>
            </div>

            <ColorPicker colors={colors} onChange={setColors} />
          </div>

          {/* Step 3: Select Style */}
          <div className="bg-white rounded-2xl shadow-lg p-6 border border-gray-100">
            <div className="flex items-center gap-3 mb-4">
              <div className="w-8 h-8 bg-pink-100 text-pink-600 rounded-full flex items-center justify-center font-bold">
                3
              </div>
              <h2 className="text-xl font-bold">Select Style</h2>
            </div>

            <StyleSelector
              selectedStyle={selectedStyle}
              onSelectStyle={setSelectedStyle}
            />
          </div>

          {/* Quality Selector */}
          <div className="bg-gradient-to-r from-yellow-50 to-orange-50 rounded-2xl p-6 border border-yellow-200">
            <div className="flex items-center justify-between mb-4">
              <div>
                <h3 className="font-semibold text-gray-900">Image Quality</h3>
                <p className="text-sm text-gray-600">HD costs 2 credits (2x better quality)</p>
              </div>
              <div className="flex gap-2">
                <button
                  onClick={() => setQuality('standard')}
                  className={`px-4 py-2 rounded-lg font-medium transition-all ${
                    quality === 'standard'
                      ? 'bg-white text-gray-900 shadow-md'
                      : 'text-gray-600 hover:text-gray-900'
                  }`}
                >
                  Standard
                </button>
                <button
                  onClick={() => setQuality('hd')}
                  className={`px-4 py-2 rounded-lg font-medium transition-all ${
                    quality === 'hd'
                      ? 'bg-gradient-to-r from-yellow-400 to-orange-400 text-white shadow-md'
                      : 'text-gray-600 hover:text-gray-900'
                  }`}
                >
                  HD
                </button>
              </div>
            </div>
          </div>

          {/* Generate Button */}
          <button
            onClick={handleGenerate}
            disabled={generateMutation.isPending || !prompt.trim()}
            className="w-full bg-gradient-to-r from-blue-600 to-purple-600 text-white font-bold py-4 px-8 rounded-xl shadow-xl hover:shadow-2xl hover:scale-105 transition-all disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:scale-100 flex items-center justify-center gap-3 text-lg"
          >
            {generateMutation.isPending ? (
              <>
                <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin" />
                Generating...
              </>
            ) : (
              <>
                <Wand2 className="w-6 h-6" />
                Generate Icon
              </>
            )}
          </button>

          <p className="text-center text-sm text-gray-500">
            {quality === 'standard' ? '1 credit' : '2 credits'} per generation
          </p>
        </div>

        {/* Right Column - Results */}
        <div className="lg:sticky lg:top-24 h-fit">
          <GenerationResults
            icon={generatedIcon}
            isGenerating={generateMutation.isPending}
          />
        </div>
      </div>

      {/* Features Section */}
      <div className="mt-20 grid grid-cols-1 md:grid-cols-3 gap-8">
        {[
          {
            icon: Sparkles,
            title: 'AI-Powered',
            description: 'Advanced DALL-E 3 technology creates unique, professional icons',
            color: 'blue',
          },
          {
            icon: Palette,
            title: 'Fully Customizable',
            description: 'Choose from 18+ styles and custom color combinations',
            color: 'purple',
          },
          {
            icon: Wand2,
            title: 'Instant Results',
            description: 'Generate production-ready icons in seconds, not hours',
            color: 'pink',
          },
        ].map((feature) => {
          const Icon = feature.icon
          return (
            <div
              key={feature.title}
              className="bg-white rounded-xl p-6 border border-gray-100 hover:shadow-lg transition-shadow"
            >
              <div className={`w-12 h-12 bg-${feature.color}-100 rounded-lg flex items-center justify-center mb-4`}>
                <Icon className={`w-6 h-6 text-${feature.color}-600`} />
              </div>
              <h3 className="text-lg font-semibold mb-2">{feature.title}</h3>
              <p className="text-gray-600 text-sm">{feature.description}</p>
            </div>
          )
        })}
      </div>
    </div>
  )
}
