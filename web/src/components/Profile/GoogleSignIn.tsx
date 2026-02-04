import { useEffect, useState } from 'react'
import toast from 'react-hot-toast'

interface GoogleSignInProps {
  onSuccess?: (response: any) => void
  onError?: (error: any) => void
  variant?: 'default' | 'large'
}

declare global {
  interface Window {
    google?: any
  }
}

export function GoogleSignIn({ onSuccess, onError, variant = 'default' }: GoogleSignInProps) {
  const [processing, setProcessing] = useState(false)

  // Listen for messages from OAuth popup
  useEffect(() => {
    const handleMessage = async (event: MessageEvent) => {
      // Verify origin for security
      if (event.origin !== window.location.origin) {
        return
      }

      if (event.data.type === 'GOOGLE_AUTH_SUCCESS') {
        console.log('✅ Received auth token from popup')
        await processAuthToken(event.data.idToken)
      } else if (event.data.type === 'GOOGLE_AUTH_ERROR') {
        console.error('❌ Auth error from popup:', event.data.error)
        toast.error('Authentication failed. Please try again.')
        setProcessing(false)
        if (onError) {
          onError(new Error(event.data.error))
        }
      }
    }

    window.addEventListener('message', handleMessage)
    return () => window.removeEventListener('message', handleMessage)
  }, [onSuccess, onError])

  const processAuthToken = async (idToken: string) => {
    setProcessing(true)
    const loadingToast = toast.loading('Signing you in...')

    try {
      console.log('📤 Sending to backend...')
      console.log('ID Token length:', idToken.length)

      const res = await fetch(`${import.meta.env.VITE_API_ENDPOINT}/auth/google`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ idToken })
      })

      console.log('📥 Response status:', res.status)

      const authResponse = await res.json()
      console.log('📦 Full response:', authResponse)

      if (!res.ok || authResponse.error) {
        console.error('❌ Auth failed:', authResponse)
        toast.error(`Authentication failed: ${authResponse.error || 'Please try again'}`, { id: loadingToast })
        setProcessing(false)
        return
      }

      console.log('✅ Auth successful!', { userId: authResponse.userId, email: authResponse.email })

      // Store everything
      localStorage.setItem('accessToken', authResponse.accessToken)
      localStorage.setItem('userId', authResponse.userId)
      localStorage.setItem('userEmail', authResponse.email)
      localStorage.setItem('userName', authResponse.name || '')
      localStorage.setItem('userPicture', authResponse.profilePictureUrl || '')

      console.log('💾 Stored in localStorage')

      toast.success(`Welcome, ${authResponse.name || 'User'}!`, { id: loadingToast })

      // Call success callback - this will update the Profile component's state
      if (onSuccess) {
        console.log('📢 Calling onSuccess callback')
        onSuccess(authResponse)
      } else {
        console.log('⚠️ No onSuccess callback provided')
      }

      setProcessing(false)

    } catch (error) {
      console.error('💥 Error:', error)
      toast.error('Authentication failed. Please try again.', { id: loadingToast })
      setProcessing(false)

      if (onError) {
        onError(error)
      }
    }
  }

  const handleSignIn = () => {
    const clientId = import.meta.env.VITE_GOOGLE_CLIENT_ID
    const redirectUri = `${window.location.origin}/oauth-callback.html`

    console.log('🚀 Starting OAuth popup flow...')
    console.log('Client ID:', clientId)
    console.log('Redirect URI:', redirectUri)

    const authUrl = `https://accounts.google.com/o/oauth2/v2/auth?` +
      `client_id=${clientId}&` +
      `redirect_uri=${encodeURIComponent(redirectUri)}&` +
      `response_type=id_token&` +
      `scope=openid%20email%20profile&` +
      `nonce=${Date.now()}`

    // Open popup window
    const width = 500
    const height = 600
    const left = window.screen.width / 2 - width / 2
    const top = window.screen.height / 2 - height / 2

    const popup = window.open(
      authUrl,
      'Google Sign In',
      `width=${width},height=${height},left=${left},top=${top},toolbar=no,menubar=no,location=no,status=no`
    )

    if (!popup) {
      toast.error('Popup blocked! Please allow popups for this site.')
      return
    }

    console.log('📍 Opened Google OAuth popup')
    setProcessing(true)

    // Check if popup was closed without completing auth
    const checkClosed = setInterval(() => {
      if (popup.closed) {
        clearInterval(checkClosed)
        if (processing) {
          console.log('⚠️ Popup closed without completing auth')
          setProcessing(false)
        }
      }
    }, 1000)
  }

  if (variant === 'large') {
    return (
      <button
        onClick={handleSignIn}
        disabled={processing}
        className="w-full group relative overflow-hidden bg-white hover:bg-gray-50 text-gray-800 font-semibold py-4 px-6 rounded-xl border-2 border-gray-200 hover:border-blue-300 transition-all duration-300 shadow-lg hover:shadow-xl disabled:opacity-50 disabled:cursor-not-allowed"
      >
        <div className="flex items-center justify-center gap-3">
          <svg className="w-6 h-6" viewBox="0 0 24 24">
            <path fill="#4285F4" d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"/>
            <path fill="#34A853" d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"/>
            <path fill="#FBBC05" d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"/>
            <path fill="#EA4335" d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"/>
          </svg>
          <span className="text-lg">{processing ? 'Processing...' : 'Continue with Google'}</span>
        </div>
        <div className="absolute inset-0 bg-gradient-to-r from-blue-50 to-purple-50 opacity-0 group-hover:opacity-100 transition-opacity duration-300 -z-10" />
      </button>
    )
  }

  return (
    <button
      onClick={handleSignIn}
      disabled={processing}
      className="flex items-center gap-3 bg-white hover:bg-gray-50 text-gray-800 font-semibold py-3 px-6 rounded-lg border-2 border-gray-200 hover:border-blue-300 transition-all duration-200 shadow-md hover:shadow-lg disabled:opacity-50 disabled:cursor-not-allowed"
    >
      <svg className="w-5 h-5" viewBox="0 0 24 24">
        <path fill="#4285F4" d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"/>
        <path fill="#34A853" d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"/>
        <path fill="#FBBC05" d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"/>
        <path fill="#EA4335" d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"/>
      </svg>
      <span>{processing ? 'Processing...' : 'Sign in with Google'}</span>
    </button>
  )
}
