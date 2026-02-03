import { Sparkles, Palette, LayoutDashboard, CreditCard } from 'lucide-react'

interface HeaderProps {
  currentPage: string
  onNavigate: (page: any) => void
}

export function Header({ currentPage, onNavigate }: HeaderProps) {
  const navItems = [
    { id: 'generator', label: 'Generator', icon: Sparkles },
    { id: 'resources', label: 'App Resources', icon: Palette },
    { id: 'dashboard', label: 'My Icons', icon: LayoutDashboard },
  ]

  return (
    <header className="sticky top-0 z-50 bg-white/90 backdrop-blur-xl border-b border-gray-200 shadow-sm">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex items-center justify-between h-16 md:h-20">
          {/* Logo */}
          <div className="flex items-center gap-2 md:gap-3 cursor-pointer group" onClick={() => onNavigate('generator')}>
            <div className="w-10 h-10 md:w-12 md:h-12 bg-gradient-to-br from-blue-600 via-purple-600 to-pink-600 rounded-xl flex items-center justify-center shadow-lg group-hover:shadow-xl group-hover:scale-105 transition-all duration-300">
              <Sparkles className="w-5 h-5 md:w-7 md:h-7 text-white" />
            </div>
            <span className="text-lg md:text-2xl font-bold bg-gradient-to-r from-blue-600 via-purple-600 to-pink-600 bg-clip-text text-transparent">
              IconGen AI
            </span>
          </div>

          {/* Navigation */}
          <nav className="hidden md:flex items-center gap-2">
            {navItems.map((item) => {
              const Icon = item.icon
              return (
                <button
                  key={item.id}
                  onClick={() => onNavigate(item.id)}
                  className={`px-4 py-2.5 rounded-xl flex items-center gap-2 font-semibold transition-all duration-300 ${
                    currentPage === item.id
                      ? 'bg-gradient-to-r from-blue-100 to-purple-100 text-blue-700 shadow-md scale-105'
                      : 'text-gray-600 hover:text-gray-900 hover:bg-gray-50 hover:scale-105'
                  }`}
                >
                  <Icon className="w-5 h-5" />
                  {item.label}
                </button>
              )
            })}
          </nav>

          {/* CTA Buttons */}
          <div className="flex items-center gap-2 md:gap-3">
            <button
              onClick={() => onNavigate('pricing')}
              className="flex items-center gap-2 px-3 md:px-4 py-2 text-sm font-semibold text-gray-700 hover:text-gray-900 transition-colors bg-gray-50 rounded-lg hover:bg-gray-100"
            >
              <CreditCard className="w-4 h-4" />
              <span className="hidden sm:inline">Credits:</span>
              <span className="font-bold text-blue-600">10</span>
            </button>

            <button
              onClick={() => onNavigate('pricing')}
              className="px-4 md:px-6 py-2 md:py-2.5 bg-gradient-to-r from-blue-600 via-purple-600 to-pink-600 text-white font-bold rounded-xl shadow-lg hover:shadow-2xl hover:scale-105 transition-all duration-300 text-sm md:text-base"
            >
              Buy Credits
            </button>
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
