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
      <div className="min-h-screen flex flex-col bg-gradient-to-br from-slate-50 via-blue-50 to-purple-50">
        <Header currentPage={currentPage} onNavigate={setCurrentPage} />

        <main className="flex-1">
          {currentPage === 'generator' && <IconGenerator />}
          {currentPage === 'resources' && <AppResources />}
          {currentPage === 'dashboard' && <Dashboard />}
          {currentPage === 'pricing' && <Pricing />}
          {currentPage === 'profile' && <Profile />}
        </main>

        <Footer />
        <Toaster position="top-right" />
      </div>
    </QueryClientProvider>
  )
}

export default App
