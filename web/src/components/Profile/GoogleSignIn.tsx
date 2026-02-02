import { useEffect } from 'react'

interface GoogleSignInProps {
  onSuccess: (response: any) => void
  onError?: (error: any) => void
}

declare global {
  interface Window {
    google?: any
  }
}

export function GoogleSignIn({ onSuccess, onError }: GoogleSignInProps) {
  useEffect(() => {
    // Load Google Sign-In script
    const script = document.createElement('script')
    script.src = 'https://accounts.google.com/gsi/client'
    script.async = true
    script.defer = true
    document.body.appendChild(script)

    script.onload = () => {
      if (window.google) {
        window.google.accounts.id.initialize({
          client_id: import.meta.env.VITE_GOOGLE_CLIENT_ID || '',
          callback: handleCredentialResponse
        })

        window.google.accounts.id.renderButton(
          document.getElementById('google-signin-button'),
          {
            theme: 'outline',
            size: 'large',
            width: 340,
            text: 'signin_with',
            shape: 'rectangular',
            logo_alignment: 'left'
          }
        )
      }
    }

    return () => {
      document.body.removeChild(script)
    }
  }, [])

  const handleCredentialResponse = async (response: any) => {
    try {
      // Send ID token to backend
      const res = await fetch(`${import.meta.env.VITE_API_ENDPOINT}/auth/google`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          idToken: response.credential
        })
      })

      if (!res.ok) {
        throw new Error('Authentication failed')
      }

      const authResponse = await res.json()
      onSuccess(authResponse)
    } catch (error) {
      console.error('Error during Google sign-in:', error)
      if (onError) {
        onError(error)
      }
    }
  }

  return (
    <div>
      <div
        id="google-signin-button"
        className="flex justify-center"
      />
    </div>
  )
}
