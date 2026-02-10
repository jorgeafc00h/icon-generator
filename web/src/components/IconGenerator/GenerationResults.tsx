import { Download, Share2, Package } from 'lucide-react'
import { useState } from 'react'
import toast from 'react-hot-toast'

interface GenerationResultsProps {
  icon: any
  isGenerating: boolean
}

export function GenerationResults({ icon, isGenerating }: GenerationResultsProps) {
  const [showEnhancedPrompt, setShowEnhancedPrompt] = useState(false)

  const handleDownload = async () => {
    if (icon?.iconId) {
      try {
        const accessToken = localStorage.getItem('accessToken')
        const apiEndpoint = import.meta.env.VITE_API_ENDPOINT

        const response = await fetch(`${apiEndpoint}/images/download/${icon.iconId}`, {
          headers: {
            'Authorization': `Bearer ${accessToken}`
          }
        })

        if (!response.ok) {
          toast.error('Failed to download icon')
          return
        }

        const blob = await response.blob()
        const url = window.URL.createObjectURL(blob)
        const link = document.createElement('a')
        link.href = url
        link.download = `icon-${icon.iconId}.png`
        document.body.appendChild(link)
        link.click()
        window.URL.revokeObjectURL(url)
        document.body.removeChild(link)
        toast.success('Downloaded!')
      } catch (error) {
        console.error('Download error:', error)
        toast.error('Failed to download icon')
      }
    }
  }

  const handleShare = () => {
    if (icon?.imageUrl) {
      navigator.clipboard.writeText(icon.imageUrl)
      toast.success('Link copied to clipboard!')
    }
  }

  if (isGenerating) {
    return (
      <div className="bg-white rounded-2xl shadow-xl p-8 border border-gray-100">
        <div className="flex flex-col items-center justify-center py-12">
          <div className="w-32 h-32 bg-gradient-to-br from-blue-500 to-purple-500 rounded-3xl animate-pulse mb-6" />
          <h3 className="text-xl font-semibold mb-2">Creating your icon...</h3>
          <p className="text-gray-500 text-center">
            Our AI is crafting something beautiful
          </p>
          <div className="mt-6 flex gap-2">
            {[...Array(3)].map((_, i) => (
              <div
                key={i}
                className="w-3 h-3 bg-blue-500 rounded-full animate-bounce"
                style={{ animationDelay: `${i * 0.15}s` }}
              />
            ))}
          </div>
        </div>
      </div>
    )
  }

  if (!icon) {
    return (
      <div className="bg-white rounded-2xl shadow-xl p-8 border border-gray-100">
        <div className="flex flex-col items-center justify-center py-12 text-center">
          <div className="w-32 h-32 bg-gradient-to-br from-gray-100 to-gray-200 rounded-3xl mb-6 flex items-center justify-center">
            <Package className="w-16 h-16 text-gray-400" />
          </div>
          <h3 className="text-xl font-semibold mb-2">Your icon will appear here</h3>
          <p className="text-gray-500 max-w-sm">
            Describe your icon, choose colors and style, then click Generate
          </p>
        </div>
      </div>
    )
  }

  return (
    <div className="bg-white rounded-2xl shadow-xl p-6 border border-gray-100 space-y-6">
      {/* Generated Icon */}
      <div className="relative">
        <div className="aspect-square bg-gradient-to-br from-gray-50 to-gray-100 rounded-2xl p-8 flex items-center justify-center">
          <img
            src={icon.imageUrl}
            alt="Generated icon"
            className="w-full h-full object-contain rounded-xl shadow-2xl"
          />
        </div>

        <div className="absolute top-4 right-4 bg-green-500 text-white px-3 py-1 rounded-full text-sm font-medium shadow-lg">
          ✓ Generated
        </div>
      </div>

      {/* Actions */}
      <div className="grid grid-cols-2 gap-3">
        <button
          onClick={handleDownload}
          className="flex items-center justify-center gap-2 px-4 py-3 bg-blue-600 text-white font-semibold rounded-lg hover:bg-blue-700 transition-colors"
        >
          <Download className="w-4 h-4" />
          Download
        </button>

        <button
          onClick={handleShare}
          className="flex items-center justify-center gap-2 px-4 py-3 bg-gray-100 text-gray-700 font-semibold rounded-lg hover:bg-gray-200 transition-colors"
        >
          <Share2 className="w-4 h-4" />
          Share
        </button>
      </div>

      {/* Enhanced Prompt */}
      {icon.enhancedPrompt && (
        <div>
          <button
            onClick={() => setShowEnhancedPrompt(!showEnhancedPrompt)}
            className="text-sm text-blue-600 hover:text-blue-700 font-medium"
          >
            {showEnhancedPrompt ? 'Hide' : 'Show'} enhanced prompt
          </button>

          {showEnhancedPrompt && (
            <div className="mt-3 p-4 bg-gray-50 rounded-lg text-sm text-gray-700 max-h-48 overflow-y-auto">
              {icon.enhancedPrompt}
            </div>
          )}
        </div>
      )}

      {/* Credits Info */}
      <div className="pt-4 border-t border-gray-200">
        <div className="flex items-center justify-between text-sm">
          <span className="text-gray-600">Credits remaining:</span>
          <span className="font-bold text-blue-600">
            {icon.creditsRemaining >= 2147483647 ? '∞ Unlimited' : icon.creditsRemaining}
          </span>
        </div>
      </div>

      {/* Generate App Resources CTA */}
      <button className="w-full bg-gradient-to-r from-purple-600 to-pink-600 text-white font-semibold py-3 px-4 rounded-lg hover:shadow-lg transition-shadow flex items-center justify-center gap-2">
        <Package className="w-5 h-5" />
        Generate App Resources
      </button>
    </div>
  )
}
