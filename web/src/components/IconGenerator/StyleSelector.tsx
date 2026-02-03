import type { IconStyle, StyleOption } from '../../types'
import { styleSamples } from '../../utils/sampleImages'
import { Check } from 'lucide-react'

interface StyleSelectorProps {
  selectedStyle: IconStyle
  onSelectStyle: (style: IconStyle) => void
}

const styles: StyleOption[] = [
  { id: '3D', name: '3D', description: 'Realistic with depth and shadows', popular: true },
  { id: 'Minimal', name: 'Minimal', description: 'Clean and simple', popular: true },
  { id: 'Gradient', name: 'Gradient', description: 'Smooth color transitions', popular: true },
  { id: 'Glassmorphism', name: 'Glass', description: 'Frosted glass effect' },
  { id: 'Neomorphism', name: 'Neomorph', description: 'Soft UI design' },
  { id: 'Clay', name: 'Clay', description: 'Playful 3D clay style', popular: true },
  { id: 'Pixel', name: 'Pixel', description: 'Retro 8-bit style' },
  { id: 'Flat', name: 'Flat', description: 'Modern flat design' },
  { id: 'Isometric', name: 'Isometric', description: '3D perspective view' },
  { id: 'Hand-drawn', name: 'Hand-drawn', description: 'Sketchy artistic style' },
  { id: 'Geometric', name: 'Geometric', description: 'Sharp geometric shapes' },
  { id: 'Abstract', name: 'Abstract', description: 'Creative and unique' },
  { id: 'Retro', name: 'Retro', description: 'Vintage aesthetic' },
  { id: 'Neon', name: 'Neon', description: 'Glowing neon lights' },
  { id: 'Watercolor', name: 'Watercolor', description: 'Painted watercolor effect' },
  { id: 'Metallic', name: 'Metallic', description: 'Shiny metal finish' },
  { id: 'Cartoon', name: 'Cartoon', description: 'Fun cartoon style' },
  { id: 'Realistic', name: 'Realistic', description: 'Photo-realistic render' },
]

export function StyleSelector({ selectedStyle, onSelectStyle }: StyleSelectorProps) {
  return (
    <div className="space-y-6">
      {/* Popular Styles First */}
      <div>
        <h3 className="text-sm font-semibold text-gray-700 mb-3 flex items-center gap-2">
          <span className="text-yellow-500">⭐</span>
          Popular Styles
        </h3>
        <div className="grid grid-cols-2 sm:grid-cols-4 gap-4">
          {styles.filter(s => s.popular).map((style) => (
            <StyleCard 
              key={style.id}
              style={style}
              isSelected={selectedStyle === style.id}
              onSelect={() => onSelectStyle(style.id)}
            />
          ))}
        </div>
      </div>

      {/* All Styles */}
      <div>
        <h3 className="text-sm font-semibold text-gray-700 mb-3">All Styles</h3>
        <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5 gap-4">
          {styles.map((style) => (
            <StyleCard 
              key={style.id}
              style={style}
              isSelected={selectedStyle === style.id}
              onSelect={() => onSelectStyle(style.id)}
            />
          ))}
        </div>
      </div>
    </div>
  )
}

function StyleCard({ style, isSelected, onSelect }: { 
  style: StyleOption
  isSelected: boolean
  onSelect: () => void 
}) {
  const sample = styleSamples[style.id]
  
  return (
    <button
      onClick={onSelect}
      className={`group flex flex-col rounded-2xl overflow-hidden transition-all duration-300 ${
        isSelected 
          ? 'ring-4 ring-blue-500 ring-offset-2 scale-[1.05] shadow-2xl' 
          : 'hover:scale-105 hover:shadow-xl'
      }`}
    >
      {/* Image Container - Full icon visibility without text overlay */}
      <div className="aspect-square relative bg-gradient-to-br from-gray-50 to-gray-100">
        <img 
          src={sample.src} 
          alt={sample.alt}
          className="w-full h-full object-cover"
        />
        
        {/* Checkmark for selected state */}
        {isSelected && (
          <div className="absolute top-3 right-3 bg-blue-500 text-white rounded-full p-1.5 shadow-lg z-10">
            <Check className="w-5 h-5" />
          </div>
        )}
        
        {/* Subtle overlay on hover only */}
        <div className={`absolute inset-0 bg-black/10 transition-opacity duration-300 ${
          isSelected ? 'opacity-0' : 'opacity-0 group-hover:opacity-100'
        }`} />
      </div>

      {/* Label - Separate from image, no overlay */}
      <div className={`p-3 text-center transition-all duration-300 ${
        isSelected 
          ? 'bg-blue-500 text-white' 
          : 'bg-white text-gray-900'
      }`}>
        <h3 className="font-bold text-base truncate">{style.name}</h3>
        <p className={`text-xs truncate mt-0.5 ${isSelected ? 'text-blue-100' : 'text-gray-500'}`}>
          {style.description}
        </p>
      </div>
    </button>
  )
}
