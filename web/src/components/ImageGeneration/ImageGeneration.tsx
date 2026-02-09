import { useState, useEffect } from 'react'
import { Sparkles, Wand2, Image as ImageIcon, Download, Loader2, Check, Palette, Info } from 'lucide-react'
import { api } from '../../services/api'
import toast from 'react-hot-toast'
import type { User, ImageStyle, GeneratedStoryImage, ImageStyleInfo } from '../../types'

interface ImageGenerationProps {
  user?: User | null
  onUserUpdate?: () => void
}

export function ImageGeneration({ user, onUserUpdate }: ImageGenerationProps) {
  const [prompt, setPrompt] = useState('')
  const [selectedStyle, setSelectedStyle] = useState<ImageStyle>('Illustration')
  const [characterDescription, setCharacterDescription] = useState('')
  const [numberOfImages, setNumberOfImages] = useState(3)
  const [isEnhancing, setIsEnhancing] = useState(false)
  const [isGenerating, setIsGenerating] = useState(false)
  const [enhancedPrompt, setEnhancedPrompt] = useState('')
  const [scenePrompts, setScenePrompts] = useState<string[]>([])
  const [generatedImages, setGeneratedImages] = useState<GeneratedStoryImage[]>([])
  const [_sessionId, setSessionId] = useState<string | null>(null)
  const [imageStyles, setImageStyles] = useState<ImageStyleInfo[]>([])

  useEffect(() => {
    loadImageStyles()
  }, [])

  const loadImageStyles = async () => {
    try {
      const styles = await api.getImageStyles()
      setImageStyles(styles)
    } catch (error) {
      console.error('Error loading image styles:', error)
    }
  }

  const handleEnhancePrompt = async () => {
    if (!prompt.trim()) {
      toast.error('Please enter a prompt first')
      return
    }

    setIsEnhancing(true)
    try {
      console.log('Sending enhance request:', {
        userPrompt: prompt.trim(),
        style: selectedStyle,
        characterDescription: characterDescription.trim() || null,
        numberOfScenes: numberOfImages
      })

      const response = await api.enhanceImagePrompt({
        userPrompt: prompt.trim(),
        style: selectedStyle,
        characterDescription: characterDescription.trim() || null,
        numberOfScenes: numberOfImages
      })

      console.log('Enhanced response:', response)

      setEnhancedPrompt(response.enhancedPrompt)
      setScenePrompts(response.scenePrompts)

      toast.success('Prompt enhanced!', {
        icon: '✨',
        duration: 3000
      })
    } catch (error: any) {
      console.error('Error enhancing prompt:', error)
      console.error('Error response:', error.response?.data)

      const errorMessage = error.response?.data?.error
        || error.response?.data?.details
        || error.message
        || 'Failed to enhance prompt'

      toast.error(errorMessage, { duration: 5000 })
    } finally {
      setIsEnhancing(false)
    }
  }

  const handleGeneratePreviews = async () => {
    if (!prompt.trim()) {
      toast.error('Please enter a prompt')
      return
    }

    if (!user) {
      toast.error('Please sign in to generate images')
      return
    }

    const creditsNeeded = Math.ceil(numberOfImages * 0.5)
    if (!(user.isUnlimited) && user.credits < creditsNeeded) {
      toast.error(`Not enough credits. You need ${creditsNeeded} credits but have ${user.credits}`)
      return
    }

    setIsGenerating(true)
    try {
      const response = await api.generatePreviewImages({
        basePrompt: prompt.trim(),
        style: selectedStyle,
        quality: 'Preview',
        numberOfImages,
        characterDescription: characterDescription.trim() || null,
        sceneDescriptions: scenePrompts.length > 0 ? scenePrompts : [],
        useEnhancedPrompts: scenePrompts.length > 0
      })

      setSessionId(response.sessionId)
      setGeneratedImages(response.images)

      if (onUserUpdate) {
        onUserUpdate()
      }

      toast.success(`Generated ${response.images.length} preview images!`, {
        icon: '🎨',
        duration: 4000
      })

      // Scroll to images
      setTimeout(() => {
        document.getElementById('generated-images')?.scrollIntoView({ behavior: 'smooth' })
      }, 100)
    } catch (error: any) {
      console.error('Error generating images:', error)
      toast.error(error.response?.data?.error || 'Failed to generate images')
    } finally {
      setIsGenerating(false)
    }
  }

  const handleDownloadImage = async (image: GeneratedStoryImage) => {
    try {
      const response = await fetch(image.imageUrl)
      const blob = await response.blob()
      const url = window.URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.href = url
      a.download = `scene-${image.sceneNumber}-${image.style}.png`
      document.body.appendChild(a)
      a.click()
      window.URL.revokeObjectURL(url)
      document.body.removeChild(a)
    } catch (error) {
      console.error('Error downloading image:', error)
      toast.error('Failed to download image')
    }
  }

  const selectedStyleInfo = imageStyles.find(s => s.id === selectedStyle)

  return (
    <div className="container mx-auto px-4 py-8 md:py-12 max-w-7xl">
      {/* Hero Section */}
      <div className="text-center mb-8 md:mb-12 animate-slide-in">
        <div className="inline-flex items-center gap-2 bg-gradient-to-r from-purple-50 to-pink-50 border border-purple-200 text-purple-700 px-4 py-2 rounded-full mb-4 md:mb-6">
          <Sparkles size={16} />
          <span className="text-sm font-medium">AI Story Illustration Generator</span>
        </div>
        <h1 className="text-3xl md:text-5xl lg:text-6xl font-bold mb-4 md:mb-6 bg-gradient-to-r from-purple-600 via-pink-600 to-orange-600 bg-clip-text text-transparent">
          Create Consistent Story Images
        </h1>
        <p className="text-lg md:text-xl text-gray-600 max-w-3xl mx-auto leading-relaxed">
          Generate multiple scenes with consistent characters and style. Perfect for children's books, comics, and visual storytelling.
        </p>
      </div>

      {/* Main Configuration */}
      <div className="card-premium rounded-2xl p-6 md:p-8 mb-8 shadow-xl">
        {/* Style Selection */}
        <div className="mb-8">
          <label className="block text-sm font-bold text-gray-900 mb-3 flex items-center gap-2">
            <Palette className="w-5 h-5 text-purple-600" />
            Art Style
          </label>
          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-3">
            {imageStyles.map((style) => (
              <button
                key={style.id}
                onClick={() => setSelectedStyle(style.id)}
                className={`
                  p-4 rounded-xl border-2 transition-all duration-300 text-left
                  ${selectedStyle === style.id
                    ? 'border-purple-600 bg-purple-50 shadow-lg scale-[1.02]'
                    : 'border-gray-200 hover:border-purple-300 hover:shadow-md hover:scale-[1.01]'}
                `}
              >
                {selectedStyle === style.id && (
                  <div className="absolute top-2 right-2">
                    <div className="w-5 h-5 bg-purple-600 rounded-full flex items-center justify-center">
                      <Check size={12} className="text-white" />
                    </div>
                  </div>
                )}
                <div className="font-bold text-sm mb-1">{style.name}</div>
                <div className="text-xs text-gray-600 line-clamp-2">{style.description}</div>
              </button>
            ))}
          </div>

          {selectedStyleInfo && (
            <div className="mt-4 p-4 bg-gradient-to-r from-purple-50 to-pink-50 rounded-xl border border-purple-200">
              <div className="flex items-start gap-2">
                <Info className="w-5 h-5 text-purple-600 flex-shrink-0 mt-0.5" />
                <div>
                  <p className="text-sm font-semibold text-purple-900 mb-1">
                    {selectedStyleInfo.name} Style
                  </p>
                  <p className="text-sm text-purple-700">
                    Example: {selectedStyleInfo.example}
                  </p>
                </div>
              </div>
            </div>
          )}
        </div>

        {/* Prompt Input */}
        <div className="mb-6">
          <label className="block text-sm font-bold text-gray-900 mb-2">
            What do you want to create?
          </label>
          <textarea
            value={prompt}
            onChange={(e) => setPrompt(e.target.value)}
            placeholder="e.g., A friendly owl sitting on a tree branch at sunset"
            className="w-full px-4 py-3 border-2 border-gray-300 rounded-xl focus:ring-4 focus:ring-purple-500 focus:ring-opacity-30 focus:border-purple-500 transition-all resize-none text-base"
            rows={3}
          />
        </div>

        {/* Character Description */}
        <div className="mb-6">
          <label className="block text-sm font-bold text-gray-900 mb-2">
            Character/Subject Description (for consistency across scenes)
          </label>
          <input
            type="text"
            value={characterDescription}
            onChange={(e) => setCharacterDescription(e.target.value)}
            placeholder="e.g., A small brown owl with big curious eyes and orange feet"
            className="w-full px-4 py-3 border-2 border-gray-300 rounded-xl focus:ring-4 focus:ring-purple-500 focus:ring-opacity-30 focus:border-purple-500 transition-all text-base"
          />
          <p className="text-xs text-gray-500 mt-2">
            This helps maintain the same character appearance across all generated images
          </p>
        </div>

        {/* Number of Images */}
        <div className="mb-6">
          <label className="block text-sm font-bold text-gray-900 mb-2">
            Number of Scenes
          </label>
          <select
            value={numberOfImages}
            onChange={(e) => setNumberOfImages(parseInt(e.target.value))}
            className="w-full px-4 py-3 border-2 border-gray-300 rounded-xl focus:ring-4 focus:ring-purple-500 focus:ring-opacity-30 focus:border-purple-500 transition-all text-base"
          >
            {[1, 2, 3, 4, 5, 6, 8, 10].map((num) => (
              <option key={num} value={num}>
                {num} {num === 1 ? 'scene' : 'scenes'}
              </option>
            ))}
          </select>
        </div>

        {/* Action Buttons */}
        <div className="flex flex-col sm:flex-row gap-3">
          <button
            onClick={handleEnhancePrompt}
            disabled={isEnhancing || !prompt.trim()}
            className="flex-1 px-6 py-3 bg-gradient-to-r from-blue-600 to-blue-700 text-white font-bold rounded-xl shadow-lg hover:shadow-xl transition-all disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
          >
            {isEnhancing ? (
              <>
                <Loader2 className="w-5 h-5 animate-spin" />
                Enhancing...
              </>
            ) : (
              <>
                <Wand2 className="w-5 h-5" />
                Enhance Prompt with AI
              </>
            )}
          </button>

          <button
            onClick={handleGeneratePreviews}
            disabled={isGenerating || !prompt.trim()}
            className="flex-1 px-6 py-3 bg-gradient-to-r from-purple-600 to-pink-600 text-white font-bold rounded-xl shadow-lg hover:shadow-xl transition-all disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
          >
            {isGenerating ? (
              <>
                <Loader2 className="w-5 h-5 animate-spin" />
                Generating...
              </>
            ) : (
              <>
                <Sparkles className="w-5 h-5" />
                Generate Preview Images
              </>
            )}
          </button>
        </div>

        {/* Cost Info */}
        <div className="mt-4 p-4 bg-gradient-to-r from-gray-50 to-gray-100 rounded-xl border border-gray-200">
          <p className="text-sm text-gray-700">
            <span className="font-bold">Preview Cost:</span> {Math.ceil(numberOfImages * 0.5)} credits (~${(Math.ceil(numberOfImages * 0.5) * 0.04).toFixed(2)}) •
            <span className="font-bold ml-2">Final HD Cost:</span> {numberOfImages} credits (~${(numberOfImages * 0.04).toFixed(2)})
          </p>
          {user && (
            <p className="text-xs text-gray-600 mt-1">
              Your balance: {user.isUnlimited ? '∞' : user.credits} credits
            </p>
          )}
        </div>
      </div>

      {/* Enhanced Prompt Display */}
      {enhancedPrompt && (
        <div className="card-premium rounded-2xl p-6 mb-8 shadow-xl animate-slide-in">
          <h3 className="font-bold text-lg mb-4 flex items-center gap-2">
            <Sparkles className="w-5 h-5 text-purple-600" />
            AI-Enhanced Prompts
          </h3>
          <div className="space-y-3">
            {scenePrompts.map((scenePrompt, index) => (
              <div key={index} className="p-3 bg-gradient-to-r from-purple-50 to-pink-50 rounded-lg border border-purple-200">
                <p className="text-sm font-semibold text-purple-900 mb-1">Scene {index + 1}</p>
                <p className="text-sm text-gray-700">{scenePrompt}</p>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Generated Images Grid */}
      {generatedImages.length > 0 && (
        <div id="generated-images" className="animate-slide-in">
          <div className="card-premium rounded-2xl p-6 md:p-8 mb-8 shadow-xl">
            <div className="flex items-center justify-between mb-6">
              <h3 className="font-bold text-2xl flex items-center gap-2">
                <ImageIcon className="w-6 h-6 text-purple-600" />
                Generated Preview Images
              </h3>
              <span className="px-4 py-2 bg-gradient-to-r from-green-50 to-emerald-50 text-green-700 font-bold rounded-xl border border-green-200">
                {generatedImages.length} scenes
              </span>
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
              {generatedImages.map((image, index) => (
                <div
                  key={image.id}
                  className="card-premium rounded-2xl p-4 hover-lift group animate-stagger"
                  style={{ animationDelay: `${index * 0.1}s` }}
                >
                  <div className="relative aspect-square rounded-xl overflow-hidden mb-4 bg-gray-100">
                    <img
                      src={image.imageUrl}
                      alt={`Scene ${image.sceneNumber}`}
                      className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500"
                    />

                    <div className="absolute inset-0 bg-gradient-to-t from-black/80 via-black/40 to-transparent opacity-0 group-hover:opacity-100 transition-all duration-300 flex items-center justify-center">
                      <button
                        onClick={() => handleDownloadImage(image)}
                        className="bg-white/95 hover:bg-white rounded-xl px-4 py-2 flex items-center gap-2 font-bold text-gray-900 shadow-lg hover:scale-110 transition-all"
                      >
                        <Download className="w-5 h-5" />
                        Download
                      </button>
                    </div>

                    <div className="absolute top-2 left-2">
                      <div className="bg-white/95 backdrop-blur-sm px-3 py-1 rounded-full text-xs font-bold text-gray-900 shadow-lg">
                        Scene {image.sceneNumber}
                      </div>
                    </div>
                  </div>

                  <p className="text-sm text-gray-600 line-clamp-2 mb-3">
                    {image.sceneDescription || image.prompt}
                  </p>

                  <button
                    onClick={() => handleDownloadImage(image)}
                    className="w-full px-3 py-2 bg-gradient-to-r from-gray-100 to-gray-50 hover:from-purple-50 hover:to-pink-50 border border-gray-200 rounded-lg text-sm font-semibold text-gray-700 hover:text-purple-600 transition-all flex items-center justify-center gap-2"
                  >
                    <Download className="w-4 h-4" />
                    Download Preview
                  </button>
                </div>
              ))}
            </div>

            {/* Future: Generate Final HD Button */}
            <div className="mt-8 p-6 bg-gradient-to-r from-blue-50 to-purple-50 rounded-2xl border border-blue-200">
              <h4 className="font-bold text-lg mb-2 text-gray-900">Ready for High-Quality?</h4>
              <p className="text-sm text-gray-600 mb-4">
                Generate final HD versions (1792x1024) of your approved previews. Cost: {generatedImages.length} credits
              </p>
              <button
                disabled
                className="px-6 py-3 bg-gradient-to-r from-purple-600 to-pink-600 text-white font-bold rounded-xl shadow-lg opacity-50 cursor-not-allowed flex items-center gap-2"
              >
                <Sparkles className="w-5 h-5" />
                Generate Final HD Images (Coming Soon)
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Empty State */}
      {generatedImages.length === 0 && !isGenerating && (
        <div className="text-center py-20 text-gray-400">
          <div className="text-8xl mb-6">🎨</div>
          <p className="text-xl font-medium text-gray-500">Create your first story image</p>
          <p className="text-sm text-gray-400 mt-2">Enter a prompt above and click "Generate Preview Images"</p>
        </div>
      )}
    </div>
  )
}
