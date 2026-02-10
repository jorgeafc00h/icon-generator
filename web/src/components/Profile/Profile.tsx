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

interface ProfileProps {
  onUserUpdate?: (user?: any) => void
}

export function Profile({ onUserUpdate }: ProfileProps) {
  const [user, setUser] = useState<User | null>(null)
  const [loading, setLoading] = useState(true)
  const [showPurchaseModal, setShowPurchaseModal] = useState(false)
  const [activeTab, setActiveTab] = useState<'overview' | 'history' | 'settings'>('overview')

  useEffect(() => {
    console.log('🔷 Profile useEffect running...')

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
    const userId = localStorage.getItem('userId')
    console.log('🔷 LocalStorage check:', {
      hasToken: !!accessToken,
      hasUserId: !!userId,
      tokenLength: accessToken?.length,
      userId: userId
    })

    if (accessToken) {
      console.log('🔷 Access token found, fetching user data...')
      fetchUserData()
    } else {
      console.log('🔷 No access token, showing login screen')
      setLoading(false)
    }
  }, [])

  const fetchUserData = async () => {
    try {
      const accessToken = localStorage.getItem('accessToken')
      const userId = localStorage.getItem('userId')
      console.log('🔷 fetchUserData called:', { hasToken: !!accessToken, userId })

      if (!accessToken || !userId) {
        console.log('🔷 Missing accessToken or userId, aborting fetch')
        setLoading(false)
        return
      }

      const apiEndpoint = import.meta.env.VITE_API_ENDPOINT
      const url = `${apiEndpoint}/users/${userId}`
      console.log('🔷 Fetching from:', url)

      const response = await fetch(url, {
        headers: {
          'Authorization': `Bearer ${accessToken}`
        }
      })

      console.log('🔷 Response status:', response.status)

      if (!response.ok) {
        if (response.status === 401) {
          console.log('🔷 Token expired (401), signing out')
          // Token expired, sign out
          handleSignOut()
          return
        }
        const errorText = await response.text()
        console.error('🔷 Failed to fetch user data:', errorText)
        throw new Error('Failed to fetch user data')
      }

      const userData = await response.json()
      console.log('🔷 User data received:', {
        email: userData.email,
        credits: userData.credits,
        hasName: !!userData.name,
        profilePictureUrl: userData.profilePictureUrl,
        hasProfilePicture: !!userData.profilePictureUrl
      })
      setUser(userData)
    } catch (error) {
      console.error('🔷 Error fetching user data:', error)
      toast.error('Failed to load user data')
    } finally {
      setLoading(false)
    }
  }

  const handleGoogleSignIn = (authResponse: any) => {
    console.log('Sign in successful:', authResponse)

    // Store all user data in localStorage (using camelCase to match API response)
    localStorage.setItem('accessToken', authResponse.accessToken)
    localStorage.setItem('userId', authResponse.userId)
    localStorage.setItem('userEmail', authResponse.email)
    localStorage.setItem('userName', authResponse.name || '')
    localStorage.setItem('userPicture', authResponse.profilePictureUrl || '')

    const userData = {
      id: authResponse.userId,
      email: authResponse.email,
      name: authResponse.name,
      profilePictureUrl: authResponse.profilePictureUrl,
      credits: authResponse.credits,
      isUnlimited: authResponse.isUnlimited || false,
      createdAt: authResponse.createdAt || new Date().toISOString(),
      updatedAt: authResponse.updatedAt || new Date().toISOString(),
      metadata: authResponse.metadata || {
        totalIconsGenerated: 0,
        totalCreditsPurchased: 0,
        totalCreditsSpent: 0
      },
      preferences: authResponse.preferences || {
        emailNotifications: true
      }
    }

    setUser(userData)

    // Notify parent component with user data to avoid unnecessary API call
    if (onUserUpdate) {
      onUserUpdate(userData)
    }
  }

  const handleSignOut = () => {
    localStorage.clear()
    setUser(null)

    // Notify parent component
    if (onUserUpdate) {
      onUserUpdate()
    }
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
    console.log('===== Profile: Rendering login UI (user is null) =====')
    return (
      <div className="min-h-screen bg-gradient-to-br from-slate-50 via-blue-50 to-purple-50 flex items-center justify-center px-4 py-12">
        <div className="max-w-2xl w-full">
          {/* Hero Card */}
          <div className="bg-white rounded-3xl shadow-2xl overflow-hidden">
            {/* Header Section with Gradient */}
            <div className="bg-gradient-to-br from-blue-600 via-purple-600 to-pink-600 px-8 py-12 text-center relative overflow-hidden">
              <div className="absolute inset-0 bg-black opacity-5"></div>
              <div className="relative z-10">
                <div className="w-24 h-24 bg-white/20 backdrop-blur-sm rounded-full flex items-center justify-center mx-auto mb-6 shadow-xl">
                  <Sparkles size={48} className="text-white" />
                </div>
                <h1 className="text-4xl md:text-5xl font-bold mb-3 text-white">
                  Welcome to IconGen AI
                </h1>
                <p className="text-xl text-white/90 max-w-md mx-auto">
                  Create stunning app icons in seconds with AI-powered design
                </p>
              </div>
            </div>

            {/* Sign In Section */}
            <div className="px-8 py-10">
              <div className="mb-8">
                <GoogleSignIn variant="large" onSuccess={handleGoogleSignIn} />
              </div>

              {/* Benefits Grid */}
              <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
                <div className="text-center p-6 rounded-xl bg-gradient-to-br from-blue-50 to-blue-100 border border-blue-200">
                  <div className="w-12 h-12 bg-blue-600 rounded-full flex items-center justify-center mx-auto mb-4">
                    <Sparkles className="w-6 h-6 text-white" />
                  </div>
                  <div className="font-bold text-2xl text-blue-600 mb-2">2 Credits</div>
                  <div className="text-sm text-gray-600">Free to start</div>
                </div>

                <div className="text-center p-6 rounded-xl bg-gradient-to-br from-purple-50 to-purple-100 border border-purple-200">
                  <div className="w-12 h-12 bg-purple-600 rounded-full flex items-center justify-center mx-auto mb-4">
                    <TrendingUp className="w-6 h-6 text-white" />
                  </div>
                  <div className="font-bold text-2xl text-purple-600 mb-2">18+ Styles</div>
                  <div className="text-sm text-gray-600">AI-powered variety</div>
                </div>

                <div className="text-center p-6 rounded-xl bg-gradient-to-br from-pink-50 to-pink-100 border border-pink-200">
                  <div className="w-12 h-12 bg-pink-600 rounded-full flex items-center justify-center mx-auto mb-4">
                    <Download className="w-6 h-6 text-white" />
                  </div>
                  <div className="font-bold text-2xl text-pink-600 mb-2">All Platforms</div>
                  <div className="text-sm text-gray-600">iOS, Android, Web</div>
                </div>
              </div>

              {/* Features List */}
              <div className="bg-gray-50 rounded-xl p-6">
                <h3 className="font-bold text-gray-900 mb-4 text-center">Everything you need to launch</h3>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                  <div className="flex items-center gap-2">
                    <div className="w-5 h-5 bg-green-500 rounded-full flex items-center justify-center flex-shrink-0">
                      <svg className="w-3 h-3 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={3} d="M5 13l4 4L19 7" />
                      </svg>
                    </div>
                    <span className="text-sm text-gray-700">1024x1024 HD quality</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <div className="w-5 h-5 bg-green-500 rounded-full flex items-center justify-center flex-shrink-0">
                      <svg className="w-3 h-3 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={3} d="M5 13l4 4L19 7" />
                      </svg>
                    </div>
                    <span className="text-sm text-gray-700">Commercial license</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <div className="w-5 h-5 bg-green-500 rounded-full flex items-center justify-center flex-shrink-0">
                      <svg className="w-3 h-3 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={3} d="M5 13l4 4L19 7" />
                      </svg>
                    </div>
                    <span className="text-sm text-gray-700">Instant generation</span>
                  </div>
                  <div className="flex items-center gap-2">
                    <div className="w-5 h-5 bg-green-500 rounded-full flex items-center justify-center flex-shrink-0">
                      <svg className="w-3 h-3 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={3} d="M5 13l4 4L19 7" />
                      </svg>
                    </div>
                    <span className="text-sm text-gray-700">No design skills needed</span>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <p className="text-center text-sm text-gray-500 mt-6">
            By signing in, you agree to our <a href="#" className="text-blue-600 hover:underline">Terms of Service</a> and <a href="#" className="text-blue-600 hover:underline">Privacy Policy</a>
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
                <>
                  <img
                    src={user.profilePictureUrl}
                    alt={user.name || user.email}
                    className="w-20 h-20 rounded-full ring-4 ring-blue-100"
                    onError={(e) => {
                      console.error('Failed to load profile picture:', user.profilePictureUrl)
                      e.currentTarget.style.display = 'none'
                      e.currentTarget.parentElement?.querySelector('.fallback-avatar')?.classList.remove('hidden')
                    }}
                    referrerPolicy="no-referrer"
                  />
                  <div className="fallback-avatar hidden w-20 h-20 bg-gradient-to-br from-blue-600 to-purple-600 rounded-full flex items-center justify-center text-white text-2xl font-bold ring-4 ring-blue-100">
                    {((user.name || user.email || 'U').charAt(0).toUpperCase())}
                  </div>
                </>
              ) : (
                <div className="w-20 h-20 bg-gradient-to-br from-blue-600 to-purple-600 rounded-full flex items-center justify-center text-white text-2xl font-bold">
                  {((user.name || user.email || 'U').charAt(0).toUpperCase())}
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
                <h2 className="text-2xl font-bold mb-4">Recent Icons</h2>
                {user.recentIcons && user.recentIcons.length > 0 ? (
                  <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-4">
                    {user.recentIcons.map((icon) => (
                      <div key={icon.id} className="group relative aspect-square rounded-xl overflow-hidden border-2 border-gray-200 hover:border-blue-400 transition-all hover:shadow-lg">
                        <img
                          src={icon.imageUrl}
                          alt={icon.prompt}
                          className="w-full h-full object-cover"
                          onError={(e) => {
                            e.currentTarget.src = 'data:image/svg+xml,%3Csvg xmlns="http://www.w3.org/2000/svg" width="100" height="100"%3E%3Crect fill="%23ddd" width="100" height="100"/%3E%3Ctext x="50%25" y="50%25" text-anchor="middle" dy=".3em" fill="%23999"%3EIcon%3C/text%3E%3C/svg%3E'
                          }}
                        />
                        <div className="absolute inset-0 bg-gradient-to-t from-black/80 via-black/20 to-transparent opacity-0 group-hover:opacity-100 transition-opacity">
                          <div className="absolute top-2 right-2">
                            <button
                              onClick={async (e) => {
                                e.preventDefault()
                                try {
                                  const accessToken = localStorage.getItem('accessToken')
                                  const apiEndpoint = import.meta.env.VITE_API_ENDPOINT

                                  const response = await fetch(`${apiEndpoint}/images/download/${icon.id}`, {
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
                                  const a = document.createElement('a')
                                  a.href = url
                                  a.download = `${icon.prompt.replace(/[^a-z0-9]/gi, '-').toLowerCase()}-${icon.id}.png`
                                  document.body.appendChild(a)
                                  a.click()
                                  window.URL.revokeObjectURL(url)
                                  document.body.removeChild(a)
                                  toast.success('Icon downloaded!')
                                } catch (error) {
                                  console.error('Download error:', error)
                                  toast.error('Failed to download icon')
                                }
                              }}
                              className="w-8 h-8 bg-white/90 hover:bg-white rounded-full flex items-center justify-center shadow-lg transition-all hover:scale-110"
                              title="Download icon"
                            >
                              <Download className="w-4 h-4 text-gray-700" />
                            </button>
                          </div>
                          <div className="absolute bottom-0 left-0 right-0 p-3">
                            <p className="text-white text-xs font-medium truncate mb-1">{icon.prompt}</p>
                            <div className="flex items-center gap-2 text-xs">
                              <span className="text-white/80 bg-white/20 px-2 py-0.5 rounded">{icon.style}</span>
                              {icon.quality === 'hd' && <span className="text-yellow-300">⚡ HD</span>}
                            </div>
                            <p className="text-white/60 text-xs mt-1">{new Date(icon.createdAt).toLocaleDateString()}</p>
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                ) : (
                  <div className="text-center py-12 text-gray-400">
                    <History size={48} className="mx-auto mb-4 opacity-50" />
                    <p>No icons generated yet</p>
                    <p className="text-sm mt-2">Start creating icons to see them here</p>
                  </div>
                )}
              </div>
            )}

            {activeTab === 'history' && (
              <div>
                <h2 className="text-2xl font-bold mb-4">Transaction History</h2>
                {user.recentTransactions && user.recentTransactions.length > 0 ? (
                  <div className="space-y-3">
                    {user.recentTransactions.map((transaction) => (
                      <div key={transaction.id} className="bg-gray-50 rounded-lg p-4 flex items-center justify-between border border-gray-200">
                        <div className="flex items-center gap-3">
                          <div className={`w-10 h-10 rounded-full flex items-center justify-center ${
                            transaction.credits > 0 ? 'bg-green-100' : 'bg-orange-100'
                          }`}>
                            {transaction.credits > 0 ? (
                              <span className="text-green-600 font-bold">+</span>
                            ) : (
                              <span className="text-orange-600 font-bold">-</span>
                            )}
                          </div>
                          <div>
                            <p className="font-medium text-gray-900">{transaction.description}</p>
                            <p className="text-sm text-gray-500">{new Date(transaction.createdAt).toLocaleString()}</p>
                          </div>
                        </div>
                        <div className="text-right">
                          <p className={`font-bold ${transaction.credits > 0 ? 'text-green-600' : 'text-orange-600'}`}>
                            {transaction.credits > 0 ? '+' : ''}{transaction.credits} 💎
                          </p>
                          {transaction.amountInCents && (
                            <p className="text-sm text-gray-500">${(transaction.amountInCents / 100).toFixed(2)}</p>
                          )}
                        </div>
                      </div>
                    ))}
                  </div>
                ) : (
                  <div className="text-center py-12 text-gray-400">
                    <CreditCard size={48} className="mx-auto mb-4 opacity-50" />
                    <p>No transactions yet</p>
                  </div>
                )}
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
