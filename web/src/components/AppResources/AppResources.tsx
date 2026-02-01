import { Package, Smartphone, Globe, Apple } from 'lucide-react'

export function AppResources() {
  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <div className="text-center mb-12">
        <h1 className="text-4xl md:text-5xl font-bold mb-4 bg-gradient-to-r from-purple-600 to-pink-600 bg-clip-text text-transparent">
          Generate App Resources
        </h1>
        <p className="text-xl text-gray-600 max-w-2xl mx-auto">
          Transform your icon into complete app resource packages for iOS, Android, Web, and macOS
        </p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-12">
        {[
          { icon: Apple, title: 'iOS Assets', desc: '15 sizes + @2x/@3x variants', color: 'blue' },
          { icon: Smartphone, title: 'Android', desc: 'Adaptive icons + all densities', color: 'green' },
          { icon: Globe, title: 'Web/PWA', desc: 'Favicons + manifest.json', color: 'purple' },
          { icon: Package, title: 'Design System', desc: 'Colors + guidelines PDF', color: 'pink' },
        ].map((platform) => {
          const Icon = platform.icon
          return (
            <div key={platform.title} className="bg-white rounded-xl p-6 border border-gray-200 text-center hover:shadow-lg transition-shadow">
              <div className={`w-16 h-16 bg-${platform.color}-100 rounded-full mx-auto mb-4 flex items-center justify-center`}>
                <Icon className={`w-8 h-8 text-${platform.color}-600`} />
              </div>
              <h3 className="font-semibold mb-2">{platform.title}</h3>
              <p className="text-sm text-gray-600">{platform.desc}</p>
            </div>
          )
        })}
      </div>

      <div className="bg-white rounded-2xl p-8 border border-gray-200 text-center">
        <Package className="w-16 h-16 text-gray-400 mx-auto mb-4" />
        <h3 className="text-xl font-semibold mb-2">Coming Soon</h3>
        <p className="text-gray-600 mb-6">Select an icon from your dashboard to generate app resources</p>
        <button disabled className="px-6 py-3 bg-gray-300 text-gray-500 rounded-lg cursor-not-allowed">
          Select Icon First
        </button>
      </div>
    </div>
  )
}
