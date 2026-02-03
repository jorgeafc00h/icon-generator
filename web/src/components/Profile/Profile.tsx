import { useState, useEffect } from 'react'
import type { User } from '../../types'
import {
  User as UserIcon,
  CreditCard,
  History,
  Settings,
  LogOut,
  Sparkles,
  TrendingUp,
  Download,
  Calendar,
  Coins
} from 'lucide-react'
import toast from 'react-hot-toast'
import { GoogleSignIn } from './GoogleSignIn'
import { PurchaseCreditsModal } from './PurchaseCreditsModal'

export function Profile() {
  const [user, setUser] = useState<User | null>(null)
  const [loading, setLoading] = useState(true)
  const [showPurchaseModal, setShowPurchaseModal] = useState(false)
  const [activeTab, setActiveTab] = useState<'overview' | 'history' | 'settings'>('overview')

  useEffect(() => {
    // Check for payment callback
    const urlParams = new URLSearchParams(window.location.search)
    const paymentStatus = urlParams.get('payment')

    if (paymentStatus === 'success') {
      toast.success('Payment successful! Credits added to your account.', {
        icon: '🎉',
        duration: 5000
      })
      // Clear URL parameters
      window.history.replaceState({}, '', window.location.pathname)
    } else if (paymentStatus === 'canceled') {
      toast.error('Payment was canceled.', {
        icon: '❌',
        duration: 4000
      })
      window.history.replaceState({}, '', window.location.pathname)
    }

    // Check if user is logged in
    const accessToken = localStorage.getItem('accessToken')
    if (accessToken) {
      fetchUserData()
    } else {
      setLoading(false)
    }
  }, [])

  const fetchUserData = async () => {
    try {
      const accessToken = localStorage.getItem('accessToken')
      const userId = localStorage.getItem('userId')

      if (!accessToken || !userId) {
        setLoading(false)
        return
      }

      const response = await fetch(`${import.meta.env.VITE_API_ENDPOINT}/users/${userId}`, {
        headers: {
          'Authorization': `Bearer ${accessToken}`
        }
      })

      if (!response.ok) {
        if (response.status === 401) {
          // Token expired, sign out
          handleSignOut()
          return
        }
        throw new Error('Failed to fetch user data')
      }

      const userData = await response.json()
      setUser(userData)
    } catch (error) {
      console.error('Error fetching user data:', error)
      toast.error('Failed to load user data')
    } finally {
      setLoading(false)
    }
  }

  const handleGoogleSignIn = (authResponse: any) => {
    localStorage.setItem('accessToken', authResponse.accessToken)
    localStorage.setItem('userId', authResponse.userId)
    setUser({
      id: authResponse.userId,
      email: authResponse.email,
      name: authResponse.name,
      profilePictureUrl: authResponse.profilePictureUrl,
      credits: authResponse.credits,
      createdAt: new Date().toISOString(),
      updatedAt: new Date().toISOString(),
      metadata: {
        totalIconsGenerated: 0,
        totalCreditsPurchased: 0,
        totalCreditsSpent: 0
      },
      preferences: {
        emailNotifications: true
      }
    })
  }

  const handleSignOut = () => {
    localStorage.removeItem('accessToken')
    localStorage.removeItem('userId')
    setUser(null)
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen">
        <div className="animate-spin rounded-full h-12 w-12 border-4 border-blue-600 border-t-transparent" />
      </div>
    )
  }

  // Not logged in
  if (!user) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50 to-purple-50 flex items-center justify-center px-4">
        <div className="max-w-md w-full">
          {/* Hero Card */}
          <div className="bg-white rounded-2xl shadow-xl p-8 text-center">
            <div className="w-20 h-20 bg-gradient-to-br from-blue-600 to-purple-600 rounded-full flex items-center justify-center mx-auto mb-6">
              <UserIcon size={40} className="text-white" />
            </div>

            <h1 className="text-3xl font-bold mb-2 bg-gradient-to-r from-blue-600 to-purple-600 bg-clip-text text-transparent">
              Welcome to Icon Generator
            </h1>
            <p className="text-gray-600 mb-8">
              Sign in to create beautiful app icons with AI
            </p>

            <GoogleSignIn onSuccess={handleGoogleSignIn} />

            <div className="mt-8 pt-8 border-t border-gray-200">
              <h3 className="font-semibold text-gray-900 mb-4">What you'll get:</h3>
              <div className="space-y-3 text-left">
                <div className="flex items-start gap-3">
                  <Sparkles className="w-5 h-5 text-blue-600 mt-0.5" />
                  <div>
                    <div className="font-medium text-gray-900">2 Free Credits</div>
                    <div className="text-sm text-gray-600">Start creating immediately</div>
                  </div>
                </div>
                <div className="flex items-start gap-3">
                  <TrendingUp className="w-5 h-5 text-blue-600 mt-0.5" />
                  <div>
                    <div className="font-medium text-gray-900">18+ AI Styles</div>
                    <div className="text-sm text-gray-600">3D, Minimal, Gradient & more</div>
                  </div>
                </div>
                <div className="flex items-start gap-3">
                  <Download className="w-5 h-5 text-blue-600 mt-0.5" />
                  <div>
                    <div className="font-medium text-gray-900">Platform Assets</div>
                    <div className="text-sm text-gray-600">iOS, Android, Web, macOS</div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <p className="text-center text-sm text-gray-500 mt-6">
            By signing in, you agree to our Terms of Service and Privacy Policy
          </p>
        </div>
      </div>
    )
  }

  // Logged in - Profile Page
  return (
    <div className="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50 to-purple-50 py-8 px-4">
      <div className="container mx-auto max-w-6xl">
        {/* Header Card */}
        <div className="bg-white rounded-2xl shadow-lg p-8 mb-6">
          <div className="flex flex-col md:flex-row items-start md:items-center justify-between gap-6">
            {/* User Info */}
            <div className="flex items-center gap-4">
              {user.profilePictureUrl ? (
                <img
                  src={user.profilePictureUrl}
                  alt={user.name || user.email}
                  className="w-20 h-20 rounded-full ring-4 ring-blue-100"
                />
              ) : (
                <div className="w-20 h-20 bg-gradient-to-br from-blue-600 to-purple-600 rounded-full flex items-center justify-center text-white text-2xl font-bold">
                  {(user.name || user.email).charAt(0).toUpperCase()}
                </div>
              )}
              <div>
                <h1 className="text-2xl font-bold text-gray-900">{user.name || 'User'}</h1>
                <p className="text-gray-600">{user.email}</p>
                <p className="text-sm text-gray-500 mt-1">
                  Member since {new Date(user.createdAt).toLocaleDateString()}
                </p>
              </div>
            </div>

            {/* Credits */}
            <div className="flex flex-col items-end gap-3">
              <div className="bg-gradient-to-r from-blue-600 to-purple-600 rounded-xl px-6 py-4 text-white">
                <div className="flex items-center gap-2 mb-1">
                  <Coins className="w-5 h-5" />
                  <span className="text-sm font-medium">Available Credits</span>
                </div>
                <div className="text-4xl font-bold">
                  {user.isUnlimited ? (
                    <span className="flex items-center gap-2">
                      <span className="text-3xl">∞</span>
                      <span className="text-2xl">Unlimited</span>
                    </span>
                  ) : (
                    user.credits
                  )}
                </div>
              </div>
              {!user.isUnlimited && (
                <button
                  onClick={() => setShowPurchaseModal(true)}
                  className="bg-white border-2 border-blue-600 text-blue-600 px-6 py-2 rounded-lg font-semibold hover:bg-blue-50 transition-colors flex items-center gap-2"
                >
                  <CreditCard size={18} />
                  Buy Credits
                </button>
              )}
            </div>
          </div>
        </div>

        {/* Stats Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
          <div className="bg-white rounded-xl shadow p-6">
            <div className="flex items-center justify-between mb-2">
              <span className="text-gray-600">Icons Generated</span>
              <Sparkles className="w-5 h-5 text-blue-600" />
            </div>
            <div className="text-3xl font-bold text-gray-900">
              {user.metadata?.totalIconsGenerated || 0}
            </div>
          </div>

          <div className="bg-white rounded-xl shadow p-6">
            <div className="flex items-center justify-between mb-2">
              <span className="text-gray-600">Credits Purchased</span>
              <TrendingUp className="w-5 h-5 text-green-600" />
            </div>
            <div className="text-3xl font-bold text-gray-900">
              {user.metadata?.totalCreditsPurchased || 0}
            </div>
          </div>

          <div className="bg-white rounded-xl shadow p-6">
            <div className="flex items-center justify-between mb-2">
              <span className="text-gray-600">Credits Spent</span>
              <Calendar className="w-5 h-5 text-purple-600" />
            </div>
            <div className="text-3xl font-bold text-gray-900">
              {user.metadata?.totalCreditsSpent || 0}
            </div>
          </div>
        </div>

        {/* Tabs */}
        <div className="bg-white rounded-2xl shadow-lg overflow-hidden">
          {/* Tab Navigation */}
          <div className="border-b border-gray-200">
            <div className="flex">
              {[
                { id: 'overview' as const, label: 'Overview', icon: UserIcon },
                { id: 'history' as const, label: 'History', icon: History },
                { id: 'settings' as const, label: 'Settings', icon: Settings },
              ].map(tab => {
                const Icon = tab.icon
                return (
                  <button
                    key={tab.id}
                    onClick={() => setActiveTab(tab.id)}
                    className={`
                      flex items-center gap-2 px-6 py-4 font-medium border-b-2 transition-colors
                      ${activeTab === tab.id
                        ? 'border-blue-600 text-blue-600'
                        : 'border-transparent text-gray-600 hover:text-gray-900'}
                    `}
                  >
                    <Icon size={18} />
                    {tab.label}
                  </button>
                )
              })}
              <div className="flex-1" />
              <button
                onClick={handleSignOut}
                className="flex items-center gap-2 px-6 py-4 text-gray-600 hover:text-red-600 transition-colors"
              >
                <LogOut size={18} />
                Sign Out
              </button>
            </div>
          </div>

          {/* Tab Content */}
          <div className="p-8">
            {activeTab === 'overview' && (
              <div>
                <h2 className="text-2xl font-bold mb-4">Recent Activity</h2>
                <div className="text-center py-12 text-gray-400">
                  <History size={48} className="mx-auto mb-4 opacity-50" />
                  <p>No recent activity</p>
                  <p className="text-sm mt-2">Start creating icons to see your history here</p>
                </div>
              </div>
            )}

            {activeTab === 'history' && (
              <div>
                <h2 className="text-2xl font-bold mb-4">Transaction History</h2>
                <div className="text-center py-12 text-gray-400">
                  <CreditCard size={48} className="mx-auto mb-4 opacity-50" />
                  <p>No transactions yet</p>
                </div>
              </div>
            )}

            {activeTab === 'settings' && (
              <div>
                <h2 className="text-2xl font-bold mb-6">Account Settings</h2>
                <div className="space-y-6 max-w-2xl">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-2">
                      Email Notifications
                    </label>
                    <label className="flex items-center gap-3 cursor-pointer">
                      <input
                        type="checkbox"
                        checked={user.preferences?.emailNotifications}
                        onChange={() => {}}
                        className="w-5 h-5 text-blue-600 rounded focus:ring-2 focus:ring-blue-500"
                      />
                      <span className="text-gray-700">
                        Receive emails about new features and updates
                      </span>
                    </label>
                  </div>

                  <div className="pt-6 border-t border-gray-200">
                    <h3 className="font-medium text-gray-900 mb-4">Danger Zone</h3>
                    <button className="text-red-600 hover:text-red-700 font-medium">
                      Delete Account
                    </button>
                  </div>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>

      {/* Purchase Credits Modal */}
      {showPurchaseModal && (
        <PurchaseCreditsModal
          onClose={() => {
            setShowPurchaseModal(false)
            // Refresh user data in case credits were added
            fetchUserData()
          }}
          currentCredits={user.credits}
        />
      )}
    </div>
  )
}
