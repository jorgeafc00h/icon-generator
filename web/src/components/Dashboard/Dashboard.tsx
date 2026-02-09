import { Image, Coins, TrendingUp, Sparkles, Package, LogIn } from 'lucide-react'
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

      {/* Stats Grid */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
        {/* Available Credits */}
        <div className="bg-gradient-to-br from-blue-50 to-blue-100 border border-blue-200 rounded-2xl p-6">
          <div className="flex items-center justify-between mb-4">
            <div className="w-12 h-12 bg-blue-600 rounded-xl flex items-center justify-center shadow-lg">
              <Coins className="w-6 h-6 text-white" />
            </div>
            {!user.isUnlimited && user.credits < 5 && (
              <span className="px-2 py-1 bg-orange-100 text-orange-700 text-xs font-semibold rounded-full">
                Low
              </span>
            )}
          </div>
          <div className="text-3xl font-bold text-blue-600 mb-1">
            {user.isUnlimited ? '∞' : user.credits}
          </div>
          <div className="text-sm text-gray-600">Available Credits</div>
          {!user.isUnlimited && user.credits < 5 && (
            <button
              onClick={() => onNavigate && onNavigate('pricing')}
              className="mt-3 w-full px-3 py-2 bg-blue-600 text-white text-sm font-semibold rounded-lg hover:bg-blue-700 transition-colors"
            >
              Buy More
            </button>
          )}
        </div>

        {/* Total Generated */}
        <div className="bg-gradient-to-br from-purple-50 to-purple-100 border border-purple-200 rounded-2xl p-6">
          <div className="w-12 h-12 bg-purple-600 rounded-xl flex items-center justify-center mb-4 shadow-lg">
            <Sparkles className="w-6 h-6 text-white" />
          </div>
          <div className="text-3xl font-bold text-purple-600 mb-1">
            {user.metadata?.totalIconsGenerated || 0}
          </div>
          <div className="text-sm text-gray-600">Icons Generated</div>
        </div>

        {/* Total Spent */}
        <div className="bg-gradient-to-br from-pink-50 to-pink-100 border border-pink-200 rounded-2xl p-6">
          <div className="w-12 h-12 bg-pink-600 rounded-xl flex items-center justify-center mb-4 shadow-lg">
            <TrendingUp className="w-6 h-6 text-white" />
          </div>
          <div className="text-3xl font-bold text-pink-600 mb-1">
            {user.metadata?.totalCreditsSpent || 0}
          </div>
          <div className="text-sm text-gray-600">Credits Used</div>
        </div>

        {/* Total Purchased */}
        <div className="bg-gradient-to-br from-green-50 to-green-100 border border-green-200 rounded-2xl p-6">
          <div className="w-12 h-12 bg-green-600 rounded-xl flex items-center justify-center mb-4 shadow-lg">
            <Package className="w-6 h-6 text-white" />
          </div>
          <div className="text-3xl font-bold text-green-600 mb-1">
            {user.metadata?.totalCreditsPurchased || 0}
          </div>
          <div className="text-sm text-gray-600">Credits Purchased</div>
        </div>
      </div>

      {/* Generated Icons Section */}
      <div className="bg-white rounded-2xl p-8 md:p-12 border border-gray-200 text-center shadow-lg">
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
    </div>
  )
}
