import { Sparkles, Palette, Image, LayoutDashboard, CreditCard, User as UserIcon } from 'lucide-react'
import type { User } from '../../types'

interface HeaderProps {
  currentPage: string
  onNavigate: (page: any) => void
  user?: User | null
}

export function Header({ currentPage, onNavigate, user }: HeaderProps) {
  const navItems = [
    { id: 'generator', label: 'Icons', icon: Sparkles },
    { id: 'resources', label: 'App UI', icon: Palette },
    { id: 'images', label: 'Story Images', icon: Image },
    { id: 'dashboard', label: 'My Work', icon: LayoutDashboard },
    { id: 'profile', label: 'Profile', icon: UserIcon },
  ]

  return (
    <header className="sticky top-0 z-50 glass-effect border-b border-white/20 shadow-medium">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex items-center justify-between h-16 md:h-20">
          {/* Logo - Enhanced with glow effect */}
          <div className="flex items-center gap-2 md:gap-3 cursor-pointer group" onClick={() => onNavigate('generator')}>
            <div className="relative">
              <div className="w-10 h-10 md:w-12 md:h-12 bg-gradient-to-br from-blue-600 via-purple-600 to-pink-600 rounded-2xl flex items-center justify-center shadow-lg group-hover:shadow-xl group-hover:scale-110 transition-all duration-300 glow-hover">
                <Sparkles className="w-5 h-5 md:w-7 md:h-7 text-white group-hover:rotate-12 transition-transform duration-300" />
              </div>
              <div className="absolute -bottom-1 -right-1 w-4 h-4 bg-gradient-to-br from-pink-400 to-purple-400 rounded-full blur-sm opacity-60 group-hover:opacity-100 transition-opacity"></div>
            </div>
            <span className="text-lg md:text-2xl font-black bg-gradient-to-r from-blue-600 via-purple-600 to-pink-600 bg-clip-text text-transparent tracking-tight">
              IconGen AI
            </span>
          </div>

          {/* Navigation - Enhanced with better hover states */}
          <nav className="hidden md:flex items-center gap-2">
            {navItems.map((item) => {
              const Icon = item.icon
              const isActive = currentPage === item.id
              return (
                <button
                  key={item.id}
                  onClick={() => onNavigate(item.id)}
                  className={`relative px-5 py-2.5 rounded-xl flex items-center gap-2.5 font-bold transition-all duration-300 group ${
                    isActive
                      ? 'bg-gradient-to-r from-blue-100 to-purple-100 text-blue-700 shadow-md scale-105'
                      : 'text-gray-600 hover:text-gray-900 hover:bg-white/60 hover:scale-105 hover:shadow-sm'
                  }`}
                >
                  <Icon className={`w-5 h-5 transition-transform ${isActive ? 'scale-110' : 'group-hover:scale-110'}`} />
                  <span className="tracking-tight">{item.label}</span>
                  {isActive && (
                    <div className="absolute -bottom-0.5 left-0 right-0 h-0.5 bg-gradient-to-r from-blue-600 to-purple-600 rounded-full"></div>
                  )}
                </button>
              )
            })}
          </nav>

          {/* CTA Buttons */}
          <div className="flex items-center gap-2 md:gap-3">
            {user ? (
              <>
                {/* Credits Display */}
                <div className="flex items-center gap-2 px-3 py-2 bg-gray-50 rounded-lg">
                  <CreditCard className="w-4 h-4 text-gray-600" />
                  <span className="hidden sm:inline text-sm text-gray-600">Credits:</span>
                  <span className="font-bold text-blue-600">
                    {user.isUnlimited ? '∞' : user.credits}
                  </span>
                </div>

                {/* Profile Picture */}
                <button
                  onClick={() => onNavigate('profile')}
                  className="flex items-center gap-2 px-2 py-2 hover:bg-gray-50 rounded-lg transition-colors"
                  title={user.name || user.email}
                >
                  {user.profilePictureUrl ? (
                    <img
                      src={user.profilePictureUrl}
                      alt={user.name || user.email}
                      className="w-8 h-8 rounded-full ring-2 ring-blue-100 hover:ring-blue-300 transition-all"
                      onError={(e) => {
                        console.error('Failed to load profile picture in header:', user.profilePictureUrl)
                        e.currentTarget.style.display = 'none'
                      }}
                      referrerPolicy="no-referrer"
                    />
                  ) : (
                    <div className="w-8 h-8 bg-gradient-to-br from-blue-600 to-purple-600 rounded-full flex items-center justify-center text-white text-sm font-bold ring-2 ring-blue-100">
                      {((user.name || user.email || 'U').charAt(0).toUpperCase())}
                    </div>
                  )}
                  <span className="hidden md:inline text-sm font-medium text-gray-700">
                    {user.name || (user.email ? user.email.split('@')[0] : 'User')}
                  </span>
                </button>
              </>
            ) : (
              <button
                onClick={() => onNavigate('profile')}
                className="px-4 md:px-6 py-2 md:py-2.5 bg-gradient-to-r from-blue-600 via-purple-600 to-pink-600 text-white font-bold rounded-xl shadow-lg hover:shadow-2xl hover:scale-105 transition-all duration-300 text-sm md:text-base"
              >
                Sign In
              </button>
            )}

            {user && !user.isUnlimited && (
              <button
                onClick={() => onNavigate('pricing')}
                className="hidden sm:block px-4 py-2 bg-gradient-to-r from-blue-600 to-purple-600 text-white font-semibold rounded-lg hover:shadow-lg transition-all text-sm"
              >
                Buy Credits
              </button>
            )}
          </div>
        </div>
      </div>

      {/* Mobile Navigation */}
      <div className="md:hidden border-t border-gray-200 bg-white/95 backdrop-blur-sm">
        <div className="flex justify-around py-2">
          {navItems.map((item) => {
            const Icon = item.icon
            return (
              <button
                key={item.id}
                onClick={() => onNavigate(item.id)}
                className={`flex flex-col items-center gap-1 px-4 py-2 rounded-xl transition-all duration-300 ${
                  currentPage === item.id
                    ? 'text-blue-600'
                    : 'text-gray-500 hover:text-gray-700'
                }`}
              >
                <Icon className="w-5 h-5" />
                <span className="text-xs font-medium">{item.label}</span>
              </button>
            )
          })}
        </div>
      </div>
    </header>
  )
}
