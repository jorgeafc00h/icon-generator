import { useState, useEffect } from 'react'
import { Download, MessageSquare, Sparkles, Grid3x3, Zap, Image as ImageIcon } from 'lucide-react'
import { ChatInterface } from './ChatInterface'
import { api } from '../../services/api'
import type { User } from '../../types'

interface GeneratedScreen {
  id: string
  screenType: string
  imageUrl: string
  prompt: string
  createdAt: string
}

interface AppResourcesResultsProps {
  sessionId: string
  screens: GeneratedScreen[]
  appName: string
  appCategory: string
  user?: User | null
  onUserUpdate?: () => void
}

export function AppResourcesResults({ sessionId, screens: initialScreens, appName, appCategory, user, onUserUpdate }: AppResourcesResultsProps) {
  const [showChat, setShowChat] = useState(false)
  const [screens, setScreens] = useState<GeneratedScreen[]>(initialScreens)

  // Update screens when initialScreens changes
  useEffect(() => {
    setScreens(initialScreens)
  }, [initialScreens])

  const handleScreenGenerated = async () => {
    try {
      // Fetch updated session data to get all screens including the newly generated one
      const sessionData = await api.getChatSession(sessionId)
      if (sessionData.generatedScreens) {
        setScreens(sessionData.generatedScreens)
      }
    } catch (error) {
      console.error('Error refreshing screens:', error)
    }
  }

  const handleDownload = async (screen: GeneratedScreen) => {
    try {
      const response = await fetch(screen.imageUrl)
      const blob = await response.blob()
      const url = window.URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.href = url
      a.download = `${appName}-${screen.screenType}-${screen.id}.png`
      document.body.appendChild(a)
      a.click()
      window.URL.revokeObjectURL(url)
      document.body.removeChild(a)
    } catch (error) {
      console.error('Error downloading image:', error)
    }
  }

  const handleDownloadAll = async () => {
    for (const screen of screens) {
      await handleDownload(screen)
      // Small delay between downloads to avoid overwhelming the browser
      await new Promise(resolve => setTimeout(resolve, 500))
    }
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      {/* Success Header */}
      <div className="text-center mb-12 animate-slide-in">
        <div className="inline-flex items-center justify-center w-20 h-20 bg-gradient-to-br from-green-400 to-emerald-500 rounded-full mb-6 shadow-lg">
          <Sparkles className="w-10 h-10 text-white" />
        </div>
        <h1 className="text-4xl md:text-5xl font-black bg-gradient-to-r from-blue-600 via-purple-600 to-pink-600 bg-clip-text text-transparent mb-4">
          Your App is Ready! 🎉
        </h1>
        <p className="text-xl text-gray-600">
          {screens.length} screens generated for <span className="font-bold text-gray-900">{appName}</span>
        </p>
      </div>

      {/* Quick Actions */}
      <div className="card-premium p-6 rounded-2xl mb-8 animate-scale-in">
        <div className="flex flex-col sm:flex-row items-center justify-between gap-4">
          <div className="flex items-center gap-3">
            <Grid3x3 className="w-6 h-6 text-blue-600" />
            <div>
              <h3 className="font-bold text-lg text-gray-900">Generated Screens</h3>
              <p className="text-sm text-gray-600">{screens.length} mockups ready to download</p>
            </div>
          </div>

          <div className="flex flex-wrap gap-3">
            <button
              onClick={() => setShowChat(!showChat)}
              className="btn-premium px-6 py-3 bg-gradient-to-r from-purple-600 to-pink-600 text-white font-bold rounded-xl shadow-lg hover:shadow-xl transition-all group"
            >
              <span className="flex items-center gap-2">
                <MessageSquare className="w-5 h-5 group-hover:scale-110 transition-transform" />
                {showChat ? 'Hide Chat' : 'Generate More Screens'}
                <Zap className="w-4 h-4" />
              </span>
            </button>

            <button
              onClick={handleDownloadAll}
              className="btn-premium px-6 py-3 bg-gradient-to-r from-blue-600 to-blue-700 text-white font-bold rounded-xl shadow-lg hover:shadow-xl transition-all group"
            >
              <span className="flex items-center gap-2">
                <Download className="w-5 h-5 group-hover:scale-110 transition-transform" />
                Download All
              </span>
            </button>
          </div>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* Screens Grid */}
        <div className={`${showChat ? 'lg:col-span-2' : 'lg:col-span-3'} transition-all duration-300`}>
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-6">
            {screens.map((screen, index) => (
              <div
                key={screen.id}
                className="card-premium rounded-2xl p-4 hover-lift animate-stagger group"
                style={{ animationDelay: `${index * 0.1}s` }}
              >
                {/* Screen Image */}
                <div className="relative aspect-[9/16] rounded-xl overflow-hidden mb-4 bg-gray-100">
                  <img
                    src={screen.imageUrl}
                    alt={screen.screenType}
                    className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500"
                  />

                  {/* Hover Overlay */}
                  <div className="absolute inset-0 bg-gradient-to-t from-black/80 via-black/40 to-transparent opacity-0 group-hover:opacity-100 transition-all duration-300 flex items-center justify-center">
                    <button
                      onClick={() => handleDownload(screen)}
                      className="bg-white/95 hover:bg-white rounded-xl px-4 py-2 flex items-center gap-2 font-bold text-gray-900 shadow-lg hover:scale-110 transition-all"
                    >
                      <Download className="w-5 h-5" />
                      Download
                    </button>
                  </div>

                  {/* Screen Type Badge */}
                  <div className="absolute top-2 left-2">
                    <div className="bg-white/95 backdrop-blur-sm px-3 py-1 rounded-full text-xs font-bold text-gray-900 shadow-lg border border-white/20">
                      {screen.screenType}
                    </div>
                  </div>
                </div>

                {/* Screen Info */}
                <div>
                  <h3 className="font-bold text-sm text-gray-900 mb-1">{screen.screenType} Screen</h3>
                  <p className="text-xs text-gray-600 line-clamp-2 mb-3">{screen.prompt}</p>

                  <button
                    onClick={() => handleDownload(screen)}
                    className="w-full px-3 py-2 bg-gradient-to-r from-gray-100 to-gray-50 hover:from-blue-50 hover:to-purple-50 border border-gray-200 rounded-lg text-sm font-semibold text-gray-700 hover:text-blue-600 transition-all flex items-center justify-center gap-2"
                  >
                    <Download className="w-4 h-4" />
                    Download
                  </button>
                </div>
              </div>
            ))}
          </div>

          {/* Empty State */}
          {screens.length === 0 && (
            <div className="card-premium p-12 rounded-2xl text-center">
              <ImageIcon className="w-16 h-16 text-gray-400 mx-auto mb-4" />
              <h3 className="text-xl font-bold text-gray-900 mb-2">No screens yet</h3>
              <p className="text-gray-600">Use the chat to generate your first screen</p>
            </div>
          )}
        </div>

        {/* Chat Interface */}
        {showChat && (
          <div className="lg:col-span-1">
            <ChatInterface
              sessionId={sessionId}
              appName={appName}
              appCategory={appCategory}
              user={user}
              onUserUpdate={onUserUpdate}
              onScreenGenerated={handleScreenGenerated}
            />
          </div>
        )}
      </div>

      {/* Info Section */}
      <div className="mt-12 card-premium p-8 rounded-2xl">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6 text-center">
          <div>
            <div className="text-3xl font-black bg-gradient-to-br from-blue-600 to-blue-700 bg-clip-text text-transparent mb-2">
              {screens.length}
            </div>
            <p className="text-sm text-gray-600 font-semibold">Screens Generated</p>
          </div>
          <div>
            <div className="text-3xl font-black bg-gradient-to-br from-purple-600 to-purple-700 bg-clip-text text-transparent mb-2">
              {appCategory}
            </div>
            <p className="text-sm text-gray-600 font-semibold">App Category</p>
          </div>
          <div>
            <div className="text-3xl font-black bg-gradient-to-br from-pink-600 to-pink-700 bg-clip-text text-transparent mb-2">
              Ready
            </div>
            <p className="text-sm text-gray-600 font-semibold">Status</p>
          </div>
        </div>
      </div>
    </div>
  )
}
