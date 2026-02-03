import { HexColorPicker } from 'react-colorful'
import { Plus, X } from 'lucide-react'
import { useState } from 'react'

interface ColorPickerProps {
  colors: string[]
  onChange: (colors: string[]) => void
}

const presetPalettes = [
  { name: 'Ocean', colors: ['#667EEA', '#764BA2'], emoji: '🌊' },
  { name: 'Sunset', colors: ['#FF6B6B', '#FFE66D'], emoji: '🌅' },
  { name: 'Forest', colors: ['#11998E', '#38EF7D'], emoji: '🌲' },
  { name: 'Fire', colors: ['#F2994A', '#EB5757'], emoji: '🔥' },
  { name: 'Royal', colors: ['#8E2DE2', '#4A00E0'], emoji: '👑' },
  { name: 'Candy', colors: ['#FF3CAC', '#784BA0', '#2B86C5'], emoji: '🍬' },
  { name: 'Mint', colors: ['#4ECDC4', '#45B7D1'], emoji: '🌿' },
  { name: 'Rose', colors: ['#FF6B9D', '#C06C84'], emoji: '🌹' },
  { name: 'Tech', colors: ['#00F5FF', '#0066FF'], emoji: '⚡' },
  { name: 'Earth', colors: ['#8B7355', '#D4A574'], emoji: '🌍' },
]

export function ColorPicker({ colors, onChange }: ColorPickerProps) {
  const [editingIndex, setEditingIndex] = useState<number | null>(null)

  const handleColorChange = (index: number, color: string) => {
    const newColors = [...colors]
    newColors[index] = color
    onChange(newColors)
  }

  const addColor = () => {
    if (colors.length < 5) {
      onChange([...colors, '#000000'])
    }
  }

  const removeColor = (index: number) => {
    if (colors.length > 1) {
      onChange(colors.filter((_, i) => i !== index))
      setEditingIndex(null)
    }
  }

  const applyPreset = (preset: string[]) => {
    onChange(preset)
    setEditingIndex(null)
  }

  return (
    <div className="space-y-4">
      {/* Color Wells */}
      <div className="flex flex-wrap gap-3">
        {colors.map((color, index) => (
          <div key={index} className="relative group">
            <button
              onClick={() => setEditingIndex(editingIndex === index ? null : index)}
              className="color-well"
              style={{ backgroundColor: color }}
              title={color}
            />
            {colors.length > 1 && (
              <button
                onClick={() => removeColor(index)}
                className="absolute -top-2 -right-2 w-5 h-5 bg-red-500 text-white rounded-full opacity-0 group-hover:opacity-100 transition-opacity shadow-lg flex items-center justify-center"
              >
                <X className="w-3 h-3" />
              </button>
            )}
          </div>
        ))}

        {colors.length < 5 && (
          <button
            onClick={addColor}
            className="w-16 h-16 rounded-full border-2 border-dashed border-gray-300 flex items-center justify-center hover:border-blue-500 hover:bg-blue-50 transition-colors"
          >
            <Plus className="w-6 h-6 text-gray-400" />
          </button>
        )}
      </div>

      {/* Color Picker */}
      {editingIndex !== null && (
        <div className="p-4 bg-gray-50 rounded-lg border border-gray-200">
          <div className="mb-3">
            <HexColorPicker
              color={colors[editingIndex]}
              onChange={(color) => handleColorChange(editingIndex, color)}
            />
          </div>
          <input
            type="text"
            value={colors[editingIndex]}
            onChange={(e) => handleColorChange(editingIndex, e.target.value)}
            className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm font-mono"
            placeholder="#000000"
          />
        </div>
      )}

      {/* Preset Palettes */}
      <div>
        <p className="text-sm font-medium text-gray-700 mb-3">✨ Quick Palettes</p>
        <div className="grid grid-cols-2 sm:grid-cols-5 gap-2">
          {presetPalettes.map((palette) => (
            <button
              key={palette.name}
              onClick={() => applyPreset(palette.colors)}
              className="group p-3 border-2 border-gray-200 rounded-xl hover:border-blue-500 hover:shadow-lg transition-all duration-200"
              title={`Apply ${palette.name} palette`}
            >
              <div className="flex gap-1 mb-2 h-8 rounded-lg overflow-hidden shadow-sm">
                {palette.colors.map((color, i) => (
                  <div
                    key={i}
                    className="flex-1"
                    style={{ backgroundColor: color }}
                  />
                ))}
              </div>
              <p className="text-xs font-medium text-gray-700 group-hover:text-blue-600 transition-colors">
                {palette.emoji} {palette.name}
              </p>
            </button>
          ))}
        </div>
      </div>
    </div>
  )
}
