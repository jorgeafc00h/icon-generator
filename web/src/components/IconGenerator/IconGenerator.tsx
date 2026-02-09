import { useState } from 'react'
import { useMutation } from '@tanstack/react-query'
import { api } from '../../services/api'
import { StyleSelector } from './StyleSelector'
import { ColorPicker} from './ColorPicker'
import { PromptInput } from './PromptInput'
import { GenerationResults } from './GenerationResults'
import { Sparkles, Wand2, Palette, AlertCircle, LogIn } from 'lucide-react'
import toast from 'react-hot-toast'
import type { IconGenerationRequest, IconStyle, User } from '../../types'

interface IconGeneratorProps {
  user?: User | null
  onUserUpdate?: () => void
  onNavigate?: (page: string) => void
}

export function IconGenerator({ user, onUserUpdate, onNavigate }: IconGeneratorProps) {
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
      
      // Refresh user data to update credits
      if (onUserUpdate) {
        onUserUpdate()
      }
    },
    onError: (error: any) => {
      const errorMessage = error.response?.data?.message || error.message || 'Failed to generate icon'
      
      if (errorMessage.toLowerCase().includes('credit')) {
        toast.error(errorMessage, { duration: 5000, icon: '💳' })
      } else if (error.response?.status === 401) {
        toast.error('Please sign in to generate icons', { icon: '🔒' })
        if (onNavigate) {
          onNavigate('profile')
        }
      } else {
        toast.error(errorMessage)
      }
    },
  })

  const handleGenerate = () => {
    // Check if user is logged in
    if (!user) {
      toast.error('Please sign in to generate icons', {
        icon: '🔒',
        duration: 4000
      })
      if (onNavigate) {
        onNavigate('profile')
      }
      return
    }

    // Check if user has enough credits
    if (!user.isUnlimited) {
      const requiredCredits = quality === 'hd' ? 2 : 1
      if (user.credits < requiredCredits) {
        toast.error(`You need ${requiredCredits} credit${requiredCredits > 1 ? 's' : ''} to generate this icon. Please purchase more credits.`, {
          icon: '💳',
          duration: 5000
        })
        if (onNavigate) {
          onNavigate('pricing')
        }
        return
      }
    }

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
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* User Status Banner - Only show if not logged in or out of credits */}
      {!user && (
        <div className="mb-6 bg-gradient-to-r from-blue-50 to-purple-50 border-2 border-blue-200 rounded-2xl p-6 flex flex-col sm:flex-row items-center justify-between gap-4">
          <div className="flex items-center gap-4">
            <div className="w-12 h-12 bg-gradient-to-br from-blue-600 to-purple-600 rounded-full flex items-center justify-center flex-shrink-0">
              <AlertCircle className="w-6 h-6 text-white" />
            </div>
            <div>
              <h3 className="font-bold text-gray-900">Sign in to generate icons</h3>
              <p className="text-sm text-gray-600">Get 2 free credits to start creating amazing icons</p>
            </div>
          </div>
          <button
            onClick={() => onNavigate && onNavigate('profile')}
            className="px-6 py-3 bg-gradient-to-r from-blue-600 to-purple-600 text-white font-semibold rounded-lg hover:shadow-lg transition-all flex items-center gap-2 whitespace-nowrap"
          >
            <LogIn className="w-5 h-5" />
            Sign In
          </button>
        </div>
      )}
      
      {user && !user.isUnlimited && user.credits === 0 && (
        <div className="mb-6 bg-gradient-to-r from-orange-50 to-red-50 border-2 border-orange-200 rounded-2xl p-6 flex flex-col sm:flex-row items-center justify-between gap-4">
          <div className="flex items-center gap-4">
            <div className="w-12 h-12 bg-gradient-to-br from-orange-600 to-red-600 rounded-full flex items-center justify-center flex-shrink-0">
              <AlertCircle className="w-6 h-6 text-white" />
            </div>
            <div>
              <h3 className="font-bold text-gray-900">Out of credits</h3>
              <p className="text-sm text-gray-600">Purchase more credits to continue generating icons</p>
            </div>
          </div>
          <button
            onClick={() => onNavigate && onNavigate('pricing')}
            className="px-6 py-3 bg-gradient-to-r from-orange-600 to-red-600 text-white font-semibold rounded-lg hover:shadow-lg transition-all whitespace-nowrap"
          >
            Buy Credits
          </button>
        </div>
      )}

      {/* Main Content - 2 Column Layout */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Left: Generation Controls */}
        <div className="lg:col-span-2 bg-white rounded-2xl shadow-lg p-6 border border-gray-100">
          {/* Header */}
          <div className="mb-6">
            <h1 className="text-3xl font-bold mb-2 bg-gradient-to-r from-blue-600 via-purple-600 to-pink-600 bg-clip-text text-transparent">
              Create Your Icon
            </h1>
            <p className="text-gray-600">Generate professional app icons with AI in seconds</p>
          </div>

          {/* Prompt Input */}
          <div className="mb-6">
            <label className="block text-sm font-semibold text-gray-700 mb-2">
              What do you want to create?
            </label>
            <PromptInput
              value={prompt}
              onChange={setPrompt}
              onGenerate={handleGenerate}
              isGenerating={generateMutation.isPending}
            />
          </div>

          {/* Style Selection */}
          <div className="mb-6">
            <label className="block text-sm font-semibold text-gray-700 mb-2">
              Select Style
            </label>
            <StyleSelector
              selectedStyle={selectedStyle}
              onSelectStyle={setSelectedStyle}
            />
          </div>

          {/* Color Selection */}
          <div className="mb-6">
            <label className="block text-sm font-semibold text-gray-700 mb-2">
              Choose Colors
            </label>
            <ColorPicker colors={colors} onChange={setColors} />
          </div>

          {/* Quality & Generate - Inline */}
          <div className="border-t pt-6">
            <div className="flex flex-col sm:flex-row items-stretch sm:items-center gap-4">
              {/* Quality Selector */}
              <div className="flex-shrink-0">
                <label className="block text-sm font-semibold text-gray-700 mb-2">Quality</label>
                <div className="flex gap-2">
                  <button
                    onClick={() => setQuality('standard')}
                    className={`px-4 py-2 rounded-lg font-medium transition-all text-sm ${
                      quality === 'standard'
                        ? 'bg-gradient-to-r from-blue-600 to-purple-600 text-white shadow-md'
                        : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                    }`}
                  >
                    Standard (1💎)
                  </button>
                  <button
                    onClick={() => setQuality('hd')}
                    className={`px-4 py-2 rounded-lg font-medium transition-all text-sm ${
                      quality === 'hd'
                        ? 'bg-gradient-to-r from-yellow-400 to-orange-400 text-white shadow-md'
                        : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                    }`}
                  >
                    HD (2💎) ⚡
                  </button>
                </div>
              </div>

              {/* Generate Button */}
              <div className="flex-1">
                <label className="block text-sm font-semibold text-gray-700 mb-2 invisible sm:visible">&nbsp;</label>
                <button
                  onClick={handleGenerate}
                  disabled={generateMutation.isPending || !prompt.trim() || !user}
                  className="w-full bg-gradient-to-r from-blue-600 via-purple-600 to-pink-600 text-white font-bold py-3 px-6 rounded-xl shadow-lg hover:shadow-xl hover:scale-[1.02] transition-all disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:scale-100 flex items-center justify-center gap-2"
                >
                  {generateMutation.isPending ? (
                    <>
                      <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin" />
                      Generating...
                    </>
                  ) : (
                    <>
                      <Wand2 className="w-5 h-5" />
                      Generate Icon
                    </>
                  )}
                </button>
              </div>
            </div>
          </div>
        </div>

        {/* Right: Preview & Credits */}
        <div className="space-y-6 lg:sticky lg:top-6 h-fit">
          {/* Credits Card */}
          {user && (
            <div className="bg-gradient-to-br from-blue-50 to-purple-50 rounded-2xl p-6 border-2 border-blue-200 shadow-lg">
              <div className="flex items-center justify-between mb-4">
                <h3 className="font-bold text-gray-900">Your Credits</h3>
                <Sparkles className="w-5 h-5 text-yellow-500" />
              </div>
              
              {user.isUnlimited ? (
                <div className="text-center py-4">
                  <div className="text-5xl font-bold bg-gradient-to-r from-yellow-400 to-orange-500 bg-clip-text text-transparent mb-2">
                    ∞
                  </div>
                  <p className="text-sm font-medium text-gray-600">Unlimited Credits</p>
                  <div className="mt-3 px-3 py-1 bg-gradient-to-r from-yellow-400 to-orange-400 text-white text-xs font-semibold rounded-full inline-block">
                    VIP ACCESS
                  </div>
                </div>
              ) : (
                <div className="text-center py-4">
                  <div className="text-5xl font-bold text-gray-900 mb-2">
                    {user.credits}
                  </div>
                  <p className="text-sm font-medium text-gray-600">
                    {user.credits === 1 ? 'Credit' : 'Credits'} Available
                  </p>
                  {user.credits < 5 && user.credits > 0 && (
                    <div className="mt-3 px-3 py-1 bg-orange-100 text-orange-700 text-xs font-semibold rounded-full inline-block">
                      Low Balance
                    </div>
                  )}
                  <button
                    onClick={() => onNavigate && onNavigate('pricing')}
                    className="mt-4 w-full px-4 py-2 bg-gradient-to-r from-blue-600 to-purple-600 text-white text-sm font-semibold rounded-lg hover:shadow-lg transition-all"
                  >
                    Buy More Credits
                  </button>
                </div>
              )}
            </div>
          )}

          {/* Preview */}
          <GenerationResults
            icon={generatedIcon}
            isGenerating={generateMutation.isPending}
          />
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
