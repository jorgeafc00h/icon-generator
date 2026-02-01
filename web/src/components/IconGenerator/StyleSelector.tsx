import type { IconStyle, StyleOption } from '../../types'

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
    <div className="grid grid-cols-2 sm:grid-cols-3 gap-3">
      {styles.map((style) => (
        <button
          key={style.id}
          onClick={() => onSelectStyle(style.id)}
          className={`style-card relative ${selectedStyle === style.id ? 'selected' : ''}`}
        >
          {style.popular && (
            <span className="absolute -top-2 -right-2 bg-gradient-to-r from-yellow-400 to-orange-400 text-white text-xs font-bold px-2 py-1 rounded-full shadow-lg">
              ★
            </span>
          )}

          <div className="h-20 bg-gradient-to-br from-gray-100 to-gray-200 rounded-lg mb-2 flex items-center justify-center">
            <span className="text-2xl font-bold text-gray-400">{style.name[0]}</span>
          </div>

          <h3 className="font-semibold text-sm mb-1">{style.name}</h3>
          <p className="text-xs text-gray-500">{style.description}</p>
        </button>
      ))}
    </div>
  )
}
