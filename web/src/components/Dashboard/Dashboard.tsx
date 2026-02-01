import { Image } from 'lucide-react'

export function Dashboard() {
  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <div className="mb-8">
        <h1 className="text-3xl font-bold mb-2">My Generated Icons</h1>
        <p className="text-gray-600">View and download your previously generated icons</p>
      </div>

      <div className="bg-white rounded-2xl p-12 border border-gray-200 text-center">
        <Image className="w-16 h-16 text-gray-400 mx-auto mb-4" />
        <h3 className="text-xl font-semibold mb-2">No icons yet</h3>
        <p className="text-gray-600 mb-6">Generate your first icon to see it here</p>
        <button className="px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors">
          Generate Icon
        </button>
      </div>
    </div>
  )
}
