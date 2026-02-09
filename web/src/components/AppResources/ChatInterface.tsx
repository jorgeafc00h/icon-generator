import { useState, useRef, useEffect } from 'react'
import { Send, Sparkles, Loader2, Image as ImageIcon, Zap, User as UserIcon, Bot } from 'lucide-react'
import { api } from '../../services/api'
import toast from 'react-hot-toast'
import type { User } from '../../types'

interface ChatMessage {
  role: 'user' | 'assistant' | 'system'
  content: string
  timestamp: string
  generatedScreen?: {
    id: string
    screenType: string
    imageUrl: string
    prompt: string
    createdAt: string
  }
}

interface ChatInterfaceProps {
  sessionId: string
  appName: string
  appCategory: string
  user?: User | null
  onUserUpdate?: () => void
  onScreenGenerated?: () => void
}

export function ChatInterface({ sessionId, appName, user, onUserUpdate, onScreenGenerated }: ChatInterfaceProps) {
  const [messages, setMessages] = useState<ChatMessage[]>([
    {
      role: 'assistant',
      content: `Hi! I'm your UI/UX designer assistant for **${appName}**. I can help you generate new screens or modify existing ones. What would you like to create?`,
      timestamp: new Date().toISOString()
    }
  ])
  const [inputMessage, setInputMessage] = useState('')
  const [isLoading, setIsLoading] = useState(false)
  const messagesEndRef = useRef<HTMLDivElement>(null)

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' })
  }

  useEffect(() => {
    scrollToBottom()
  }, [messages])

  const handleSend = async () => {
    if (!inputMessage.trim() || isLoading) return

    const userMessage = inputMessage.trim()
    setInputMessage('')

    // Add user message to chat
    const newUserMessage: ChatMessage = {
      role: 'user',
      content: userMessage,
      timestamp: new Date().toISOString()
    }

    setMessages(prev => [...prev, newUserMessage])
    setIsLoading(true)

    try {
      const response = await api.chatGenerateScreen({
        sessionId,
        message: userMessage
      })

      // Add assistant response
      const assistantMessage: ChatMessage = {
        role: 'assistant',
        content: response.message,
        timestamp: new Date().toISOString(),
        generatedScreen: response.generatedScreen
      }

      setMessages(prev => [...prev, assistantMessage])

      // Show success toast if screen was generated
      if (response.generatedScreen) {
        toast.success(`Generated ${response.generatedScreen.screenType} screen!`, {
          icon: '🎨',
          duration: 4000
        })

        // Update user credits
        if (onUserUpdate) {
          onUserUpdate()
        }

        // Notify parent
        if (onScreenGenerated) {
          onScreenGenerated()
        }
      }

      // Check if credits are low
      if (response.remainingCredits >= 0 && response.remainingCredits < 5) {
        toast.error(`Low credits: ${response.remainingCredits} remaining`, {
          icon: '⚠️',
          duration: 5000
        })
      }
    } catch (error: any) {
      console.error('Chat error:', error)

      const errorMessage = error.response?.data?.error || error.message || 'Failed to send message'

      // Add error message to chat
      setMessages(prev => [...prev, {
        role: 'assistant',
        content: `Sorry, I encountered an error: ${errorMessage}. Please try again.`,
        timestamp: new Date().toISOString()
      }])

      toast.error(errorMessage)
    } finally {
      setIsLoading(false)
    }
  }

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault()
      handleSend()
    }
  }

  const formatTime = (timestamp: string) => {
    const date = new Date(timestamp)
    return date.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' })
  }

  return (
    <div className="card-premium rounded-2xl flex flex-col h-[600px] shadow-xl">
      {/* Chat Header */}
      <div className="p-4 border-b border-gray-200/50 flex items-center gap-3">
        <div className="w-10 h-10 bg-gradient-to-br from-purple-500 to-pink-500 rounded-xl flex items-center justify-center shadow-lg">
          <Sparkles className="w-5 h-5 text-white" />
        </div>
        <div className="flex-1">
          <h3 className="font-bold text-gray-900">AI Designer Chat</h3>
          <p className="text-xs text-gray-600">Generate more screens on demand</p>
        </div>
        {user && !user.isUnlimited && (
          <div className="bg-gradient-to-br from-blue-50 to-purple-50 px-3 py-1.5 rounded-lg border border-blue-200">
            <p className="text-xs font-bold text-blue-700">{user.credits} credits</p>
          </div>
        )}
      </div>

      {/* Messages */}
      <div className="flex-1 overflow-y-auto p-4 space-y-4">
        {messages.filter(m => m.role !== 'system').map((message, index) => (
          <div
            key={index}
            className={`flex gap-3 ${message.role === 'user' ? 'flex-row-reverse' : 'flex-row'}`}
          >
            {/* Avatar */}
            <div className={`flex-shrink-0 w-8 h-8 rounded-full flex items-center justify-center ${
              message.role === 'user'
                ? 'bg-gradient-to-br from-blue-500 to-blue-600'
                : 'bg-gradient-to-br from-purple-500 to-pink-500'
            } shadow-md`}>
              {message.role === 'user' ? (
                <UserIcon className="w-4 h-4 text-white" />
              ) : (
                <Bot className="w-4 h-4 text-white" />
              )}
            </div>

            {/* Message Content */}
            <div className={`flex-1 ${message.role === 'user' ? 'text-right' : 'text-left'}`}>
              <div className={`inline-block p-3 rounded-xl ${
                message.role === 'user'
                  ? 'bg-gradient-to-br from-blue-600 to-blue-700 text-white shadow-md'
                  : 'bg-white border border-gray-200 text-gray-900 shadow-sm'
              } max-w-[85%]`}>
                <p className="text-sm whitespace-pre-wrap break-words">{message.content}</p>
              </div>

              {/* Generated Screen Preview */}
              {message.generatedScreen && (
                <div className="mt-3 inline-block max-w-[85%]">
                  <div className="bg-white rounded-xl border border-gray-200 p-3 shadow-md hover:shadow-lg transition-shadow">
                    <div className="aspect-[9/16] rounded-lg overflow-hidden mb-2 bg-gray-100">
                      <img
                        src={message.generatedScreen.imageUrl}
                        alt={message.generatedScreen.screenType}
                        className="w-full h-full object-cover"
                      />
                    </div>
                    <div className="flex items-center justify-between">
                      <div className="flex items-center gap-2">
                        <ImageIcon className="w-4 h-4 text-purple-600" />
                        <span className="text-xs font-bold text-gray-900">
                          {message.generatedScreen.screenType}
                        </span>
                      </div>
                      <Zap className="w-4 h-4 text-yellow-500" />
                    </div>
                  </div>
                </div>
              )}

              <p className="text-xs text-gray-500 mt-1">{formatTime(message.timestamp)}</p>
            </div>
          </div>
        ))}

        {/* Loading Indicator */}
        {isLoading && (
          <div className="flex gap-3">
            <div className="flex-shrink-0 w-8 h-8 rounded-full bg-gradient-to-br from-purple-500 to-pink-500 flex items-center justify-center shadow-md">
              <Bot className="w-4 h-4 text-white" />
            </div>
            <div className="bg-white border border-gray-200 rounded-xl p-3 shadow-sm">
              <div className="flex items-center gap-2">
                <Loader2 className="w-4 h-4 text-purple-600 animate-spin" />
                <span className="text-sm text-gray-600">Thinking...</span>
              </div>
            </div>
          </div>
        )}

        <div ref={messagesEndRef} />
      </div>

      {/* Input */}
      <div className="p-4 border-t border-gray-200/50">
        <div className="flex gap-2">
          <textarea
            value={inputMessage}
            onChange={(e) => setInputMessage(e.target.value)}
            onKeyPress={handleKeyPress}
            placeholder="Describe the screen you want to generate..."
            className="flex-1 px-4 py-3 border-2 border-gray-300 rounded-xl focus:ring-4 focus:ring-purple-500 focus:ring-opacity-30 focus:border-purple-500 transition-all resize-none text-sm"
            rows={2}
            disabled={isLoading}
          />
          <button
            onClick={handleSend}
            disabled={!inputMessage.trim() || isLoading}
            className="btn-premium px-4 py-3 bg-gradient-to-r from-purple-600 to-pink-600 text-white font-bold rounded-xl shadow-lg hover:shadow-xl transition-all disabled:opacity-50 disabled:cursor-not-allowed self-end"
          >
            {isLoading ? (
              <Loader2 className="w-5 h-5 animate-spin" />
            ) : (
              <Send className="w-5 h-5" />
            )}
          </button>
        </div>

        {/* Suggestions */}
        <div className="mt-3 flex flex-wrap gap-2">
          {[
            'Create a settings screen',
            'Add a profile page',
            'Design a dashboard',
            'Generate checkout flow'
          ].map((suggestion) => (
            <button
              key={suggestion}
              onClick={() => setInputMessage(suggestion)}
              className="px-3 py-1.5 bg-gradient-to-r from-gray-100 to-gray-50 hover:from-purple-50 hover:to-pink-50 border border-gray-200 hover:border-purple-300 rounded-lg text-xs font-semibold text-gray-700 hover:text-purple-600 transition-all"
              disabled={isLoading}
            >
              {suggestion}
            </button>
          ))}
        </div>
      </div>
    </div>
  )
}
