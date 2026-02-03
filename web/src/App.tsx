import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { Toaster } from 'react-hot-toast'
import { useState } from 'react'
import { Header } from './components/Layout/Header'
import { Footer } from './components/Layout/Footer'
import { IconGenerator } from './components/IconGenerator/IconGenerator'
import { AppResources } from './components/AppResources/AppResources'
import { Dashboard } from './components/Dashboard/Dashboard'
import { Pricing } from './components/Pricing/Pricing'
import { Profile } from './components/Profile/Profile'

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry: 1,
    },
  },
})

type Page = 'generator' | 'resources' | 'dashboard' | 'pricing' | 'profile'

function App() {
  const [currentPage, setCurrentPage] = useState<Page>('generator')

  return (
    <QueryClientProvider client={queryClient}>
      <div className="min-h-screen flex flex-col bg-gradient-to-br from-slate-50 via-blue-50 to-purple-50 relative overflow-hidden">
        {/* Animated background elements */}
        <div className="absolute inset-0 overflow-hidden pointer-events-none">
          <div className="absolute top-0 left-1/4 w-96 h-96 bg-blue-200 rounded-full mix-blend-multiply filter blur-3xl opacity-20 animate-float"></div>
          <div className="absolute top-1/3 right-1/4 w-96 h-96 bg-purple-200 rounded-full mix-blend-multiply filter blur-3xl opacity-20 animate-float" style={{ animationDelay: '1s' }}></div>
          <div className="absolute bottom-0 left-1/3 w-96 h-96 bg-pink-200 rounded-full mix-blend-multiply filter blur-3xl opacity-20 animate-float" style={{ animationDelay: '2s' }}></div>
        </div>

        <div className="relative z-10 flex flex-col min-h-screen">
          <Header currentPage={currentPage} onNavigate={setCurrentPage} />

          <main className="flex-1 animate-slide-in">
            {currentPage === 'generator' && <IconGenerator />}
            {currentPage === 'resources' && <AppResources />}
            {currentPage === 'dashboard' && <Dashboard />}
            {currentPage === 'pricing' && <Pricing />}
            {currentPage === 'profile' && <Profile />}
          </main>

          <Footer />
        </div>
        <Toaster 
          position="top-right"
          toastOptions={{
            className: 'animate-scale-in',
            duration: 4000,
            style: {
              background: '#fff',
              color: '#333',
              boxShadow: '0 10px 25px rgba(0,0,0,0.1)',
              borderRadius: '12px',
              padding: '16px',
            },
            success: {
              iconTheme: {
                primary: '#10b981',
                secondary: '#fff',
              },
            },
            error: {
              iconTheme: {
                primary: '#ef4444',
                secondary: '#fff',
              },
            },
          }}
        />
      </div>
    </QueryClientProvider>
  )
}

export default App
