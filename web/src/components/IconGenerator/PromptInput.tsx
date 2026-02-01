import { Lightbulb } from 'lucide-react'
import { useState } from 'react'

interface PromptInputProps {
  value: string
  onChange: (value: string) => void
  onGenerate: () => void
  isGenerating?: boolean
}

const examplePrompts = [
  'fitness tracker app icon',
  'music streaming service',
  'food delivery app',
  'meditation and wellness',
  'travel planning tool',
  'photo editing software',
]

export function PromptInput({ value, onChange, onGenerate }: PromptInputProps) {
  const [showSuggestions, setShowSuggestions] = useState(false)

  const handleExample = (example: string) => {
    onChange(example)
    setShowSuggestions(false)
  }

  return (
    <div className="space-y-3">
      <div className="relative">
        <textarea
          value={value}
          onChange={(e) => onChange(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === 'Enter' && (e.metaKey || e.ctrlKey)) {
              e.preventDefault()
              onGenerate()
            }
          }}
          placeholder="E.g., fitness tracker app with heart rate monitoring..."
          className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent resize-none"
          rows={4}
          maxLength={200}
        />
        <div className="absolute bottom-3 right-3 text-xs text-gray-400">
          {value.length}/200
        </div>
      </div>

      <button
        onClick={() => setShowSuggestions(!showSuggestions)}
        className="flex items-center gap-2 text-sm text-blue-600 hover:text-blue-700 font-medium"
      >
        <Lightbulb className="w-4 h-4" />
        {showSuggestions ? 'Hide examples' : 'Show examples'}
      </button>

      {showSuggestions && (
        <div className="grid grid-cols-2 gap-2">
          {examplePrompts.map((example) => (
            <button
              key={example}
              onClick={() => handleExample(example)}
              className="text-left px-3 py-2 text-sm bg-gray-50 hover:bg-gray-100 rounded-lg transition-colors border border-gray-200"
            >
              {example}
            </button>
          ))}
        </div>
      )}

      <div className="flex items-center gap-2 text-xs text-gray-500">
        <span className="inline-flex items-center gap-1 bg-gray-100 px-2 py-1 rounded">
          <kbd className="font-mono">⌘</kbd>
          <kbd className="font-mono">Enter</kbd>
        </span>
        <span>to generate</span>
      </div>
    </div>
  )
}
