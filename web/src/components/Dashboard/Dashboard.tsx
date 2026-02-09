import { Image, Coins, TrendingUp, Sparkles, Package, LogIn, Download, Zap } from 'lucide-react'
import type { User } from '../../types'

interface DashboardProps {
  user?: User | null
  onNavigate?: (page: string) => void
}

export function Dashboard({ user, onNavigate }: DashboardProps) {
  // If not logged in, show sign in prompt
  if (!user) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
        <div className="bg-gradient-to-br from-blue-50 to-purple-50 border-2 border-blue-200 rounded-3xl p-12 text-center">
          <div className="w-20 h-20 bg-gradient-to-br from-blue-600 to-purple-600 rounded-full flex items-center justify-center mx-auto mb-6">
            <LogIn className="w-10 h-10 text-white" />
          </div>
          <h2 className="text-3xl font-bold mb-4 text-gray-900">Sign in to view your icons</h2>
          <p className="text-gray-600 mb-8 max-w-md mx-auto">
            Create an account to start generating professional app icons and access your dashboard
          </p>
          <button
            onClick={() => onNavigate && onNavigate('profile')}
            className="px-8 py-4 bg-gradient-to-r from-blue-600 to-purple-600 text-white font-bold rounded-xl shadow-lg hover:shadow-2xl hover:scale-105 transition-all"
          >
            Sign In / Sign Up
          </button>
        </div>
      </div>
    )
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      {/* Header */}
      <div className="mb-8">
        <h1 className="text-3xl font-bold mb-2">Dashboard</h1>
        <p className="text-gray-600">Overview of your icon generation activity</p>
      </div>

      {/* Stats Grid - Enhanced with premium cards */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        {/* Available Credits */}
        <div className="card-premium rounded-2xl p-6 hover-lift animate-stagger group">
          <div className="flex items-center justify-between mb-4">
            <div className="relative">
              <div className="w-14 h-14 bg-gradient-to-br from-blue-500 to-blue-600 rounded-2xl flex items-center justify-center shadow-lg group-hover:shadow-xl transition-all group-hover:scale-110 duration-300">
                <Coins className="w-7 h-7 text-white" />
              </div>
              <div className="absolute -bottom-1 -right-1 w-5 h-5 bg-blue-400 rounded-full blur-sm opacity-60"></div>
            </div>
            {!user.isUnlimited && user.credits < 5 && (
              <span className="px-3 py-1.5 bg-gradient-to-r from-orange-100 to-orange-50 text-orange-700 text-xs font-bold rounded-full shadow-sm border border-orange-200 animate-pulse-ring">
                Low
              </span>
            )}
          </div>
          <div className="text-4xl font-black bg-gradient-to-br from-blue-600 to-blue-700 bg-clip-text text-transparent mb-2 tracking-tight">
            {user.isUnlimited ? '∞' : user.credits}
          </div>
          <div className="text-sm font-semibold text-gray-600 mb-3">Available Credits</div>
          {!user.isUnlimited && user.credits < 5 && (
            <button
              onClick={() => onNavigate && onNavigate('pricing')}
              className="mt-2 w-full px-4 py-2.5 bg-gradient-to-r from-blue-600 to-blue-700 text-white text-sm font-bold rounded-xl hover:shadow-lg transition-all hover:scale-105 active:scale-95"
            >
              Buy More
            </button>
          )}
        </div>

        {/* Total Generated */}
        <div className="card-premium rounded-2xl p-6 hover-lift animate-stagger group">
          <div className="relative mb-4">
            <div className="w-14 h-14 bg-gradient-to-br from-purple-500 to-purple-600 rounded-2xl flex items-center justify-center shadow-lg group-hover:shadow-xl transition-all group-hover:scale-110 group-hover:rotate-6 duration-300">
              <Sparkles className="w-7 h-7 text-white" />
            </div>
            <div className="absolute -bottom-1 -right-1 w-5 h-5 bg-purple-400 rounded-full blur-sm opacity-60"></div>
          </div>
          <div className="text-4xl font-black bg-gradient-to-br from-purple-600 to-purple-700 bg-clip-text text-transparent mb-2 tracking-tight">
            {user.metadata?.totalIconsGenerated || 0}
          </div>
          <div className="text-sm font-semibold text-gray-600">Icons Generated</div>
        </div>

        {/* Total Spent */}
        <div className="card-premium rounded-2xl p-6 hover-lift animate-stagger group">
          <div className="relative mb-4">
            <div className="w-14 h-14 bg-gradient-to-br from-pink-500 to-pink-600 rounded-2xl flex items-center justify-center shadow-lg group-hover:shadow-xl transition-all group-hover:scale-110 group-hover:-rotate-6 duration-300">
              <TrendingUp className="w-7 h-7 text-white" />
            </div>
            <div className="absolute -bottom-1 -right-1 w-5 h-5 bg-pink-400 rounded-full blur-sm opacity-60"></div>
          </div>
          <div className="text-4xl font-black bg-gradient-to-br from-pink-600 to-pink-700 bg-clip-text text-transparent mb-2 tracking-tight">
            {user.metadata?.totalCreditsSpent || 0}
          </div>
          <div className="text-sm font-semibold text-gray-600">Credits Used</div>
        </div>

        {/* Total Purchased */}
        <div className="card-premium rounded-2xl p-6 hover-lift animate-stagger group">
          <div className="relative mb-4">
            <div className="w-14 h-14 bg-gradient-to-br from-emerald-500 to-emerald-600 rounded-2xl flex items-center justify-center shadow-lg group-hover:shadow-xl transition-all group-hover:scale-110 duration-300">
              <Package className="w-7 h-7 text-white" />
            </div>
            <div className="absolute -bottom-1 -right-1 w-5 h-5 bg-emerald-400 rounded-full blur-sm opacity-60"></div>
          </div>
          <div className="text-4xl font-black bg-gradient-to-br from-emerald-600 to-emerald-700 bg-clip-text text-transparent mb-2 tracking-tight">
            {user.metadata?.totalCreditsPurchased || 0}
          </div>
          <div className="text-sm font-semibold text-gray-600">Credits Purchased</div>
        </div>
      </div>

      {/* Generated Icons Section - Enhanced */}
      <div className="card-premium rounded-3xl p-8 shadow-medium animate-slide-in">
        <div className="flex items-center justify-between mb-8">
          <div>
            <h2 className="text-3xl font-bold bg-gradient-to-r from-gray-900 to-gray-700 bg-clip-text text-transparent mb-2">Recent Icons</h2>
            <p className="text-gray-600 text-sm">Your latest creations</p>
          </div>
          <button
            onClick={() => onNavigate && onNavigate('generator')}
            className="btn-premium px-6 py-3 bg-gradient-to-r from-blue-600 via-purple-600 to-pink-600 text-white font-bold rounded-xl shadow-lg hover:shadow-xl transition-all group"
          >
            <span className="flex items-center gap-2">
              <Sparkles className="w-5 h-5 group-hover:rotate-12 transition-transform" />
              Generate New
            </span>
          </button>
        </div>

        {!user.recentIcons || user.recentIcons.length === 0 ? (
          <div className="text-center py-12">
            <Image className="w-16 h-16 text-gray-400 mx-auto mb-4" />
            <h3 className="text-xl font-semibold mb-2">No icons yet</h3>
            <p className="text-gray-600 mb-6">Generate your first icon to see it here</p>
            <button 
              onClick={() => onNavigate && onNavigate('generator')}
              className="px-6 py-3 bg-gradient-to-r from-blue-600 to-purple-600 text-white font-bold rounded-lg hover:shadow-lg hover:scale-105 transition-all"
            >
              Generate Icon
            </button>
          </div>
        ) : (
          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-6">
            {user.recentIcons.map((icon, index) => (
              <div
                key={icon.id}
                className="group relative aspect-square rounded-2xl overflow-hidden border border-gray-200/50 bg-white hover:border-blue-400 transition-all hover:shadow-xl hover:-translate-y-2 duration-300 animate-stagger"
                style={{ animationDelay: `${index * 0.05}s` }}
              >
                {/* Icon Image */}
                <img
                  src={icon.imageUrl}
                  alt={icon.prompt}
                  className="w-full h-full object-cover transition-transform duration-500 group-hover:scale-110"
                  onError={(e) => {
                    e.currentTarget.src = 'data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" width="100" height="100"%3E%3Crect fill="%23ddd" width="100" height="100"/%3E%3Ctext x="50%25" y="50%25" text-anchor="middle" dy=".3em" fill="%23999"%3EIcon%3C/text%3E%3C/svg%3E'
                  }}
                />

                {/* Hover Overlay with enhanced glassmorphism */}
                <div className="absolute inset-0 bg-gradient-to-t from-black/90 via-black/40 to-transparent opacity-0 group-hover:opacity-100 transition-all duration-300 backdrop-blur-sm">
                  {/* Download Button */}
                  <div className="absolute top-3 right-3">
                    <a
                      href={icon.imageUrl}
                      download={`icon-${icon.id}.png`}
                      onClick={(e) => {
                        e.preventDefault()
                        fetch(icon.imageUrl)
                          .then(res => res.blob())
                          .then(blob => {
                            const url = window.URL.createObjectURL(blob)
                            const a = document.createElement('a')
                            a.href = url
                            a.download = `${icon.prompt.replace(/[^a-z0-9]/gi, '-').toLowerCase()}-${icon.id}.png`
                            document.body.appendChild(a)
                            a.click()
                            window.URL.revokeObjectURL(url)
                            document.body.removeChild(a)
                          })
                      }}
                      className="w-10 h-10 bg-white/95 hover:bg-white rounded-xl flex items-center justify-center shadow-lg transition-all hover:scale-110 active:scale-95 group/btn"
                      title="Download icon"
                    >
                      <Download className="w-5 h-5 text-gray-700 group-hover/btn:text-blue-600 transition-colors" />
                    </a>
                  </div>

                  {/* Info Panel */}
                  <div className="absolute bottom-0 left-0 right-0 p-4">
                    <p className="text-white text-sm font-semibold truncate mb-2 drop-shadow-lg">{icon.prompt}</p>
                    <div className="flex items-center gap-2">
                      <span className="text-white/90 text-xs font-medium bg-white/25 backdrop-blur-sm px-3 py-1 rounded-full border border-white/20">
                        {icon.style}
                      </span>
                      {icon.quality === 'hd' && (
                        <span className="text-xs font-bold bg-gradient-to-r from-yellow-400 to-orange-400 text-white px-3 py-1 rounded-full shadow-lg flex items-center gap-1">
                          <Zap className="w-3 h-3" />
                          HD
                        </span>
                      )}
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  )
}
